﻿using NetCasbin.Model;
using NetCasbin.Persist;
using NetCasbin.Util;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RBACExample.RBAC
{
    public class CasbinFileAdapter: IAdapter
    {
        protected readonly string filePath;
        private readonly bool _readOnly;
        private readonly StreamReader _byteArrayInputStream;

        public CasbinFileAdapter(string filePath)
        {
            this.filePath = filePath;
        }

        public CasbinFileAdapter(Stream inputStream)
        {
            _readOnly = true;
            try
            {
                _byteArrayInputStream = new StreamReader(inputStream);
            }
            catch (IOException e)
            {
                throw new IOException("File adapter init error", e);
            }
        }

        public void LoadPolicy(Model model)
        {
            if (!string.IsNullOrWhiteSpace(filePath))
            {
                using (var sr = new StreamReader(new FileStream(
                    filePath, FileMode.Open, FileAccess.Read, FileShare.Read)))
                {
                    LoadPolicyData(model, Helper.LoadPolicyLine, sr);
                }
            }

            if (_byteArrayInputStream != null)
            {
                LoadPolicyData(model, Helper.LoadPolicyLine, _byteArrayInputStream);
            }
        }

        public async Task LoadPolicyAsync(Model model)
        {
            if (!string.IsNullOrWhiteSpace(filePath))
            {
                using (var sr = new StreamReader(new FileStream(
                    filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite)))
                {
                    await LoadPolicyDataAsync(model, Helper.LoadPolicyLine, sr);
                }
            }

            if (_byteArrayInputStream != null)
            {
                await LoadPolicyDataAsync(model, Helper.LoadPolicyLine, _byteArrayInputStream);
            }
        }

        public void RemoveAndSavePolicy(Model model, List<string> removePolicies)
        {
            if (_byteArrayInputStream != null && _readOnly)
            {
                throw new Exception("Policy file can not write, because use inputStream is readOnly");
            }

            if (string.IsNullOrWhiteSpace(filePath))
            {
                throw new ArgumentException("Invalid file path, file path cannot be empty");
            }

            var policy = ConvertToPolicyStrings(model);
            foreach (var p in removePolicies)
            {
                if (p.StartsWith("p"))
                {
                    policy.RemoveAt(policy.IndexOf(p));
                }
            }

            SavePolicyFile(string.Join("\n", policy));
        }

        public void SavePolicy(Model model)
        {
            if (_byteArrayInputStream != null && _readOnly)
            {
                throw new Exception("Policy file can not write, because use inputStream is readOnly");
            }

            if (string.IsNullOrWhiteSpace(filePath))
            {
                throw new ArgumentException("Invalid file path, file path cannot be empty");
            }

            var policy = ConvertToPolicyStrings(model);
            SavePolicyFile(string.Join("\n", policy));
        }

        public async Task SavePolicyAsync(Model model)
        {
            if (_byteArrayInputStream != null && _readOnly)
            {
                throw new Exception("Policy file can not write, because use inputStream is readOnly");
            }

            if (string.IsNullOrWhiteSpace(filePath))
            {
                throw new ArgumentException("Invalid file path, file path cannot be empty");
            }

            var policy = ConvertToPolicyStrings(model);
            await SavePolicyFileAsync(string.Join("\n", policy));
        }

        private static IEnumerable<string> GetModelPolicy(Model model, string ptype)
        {
            var policy = new List<string>();
            foreach (var pair in model.Model[ptype])
            {
                string key = pair.Key;
                Assertion value = pair.Value;
                policy.AddRange(value.Policy.Select(p => $"{key}, {Utility.RuleToString(p)}"));
            }
            return policy;
        }

        private static void LoadPolicyData(Model model, Helper.LoadPolicyLineHandler<string, Model> handler, StreamReader inputStream)
        {
            while (!inputStream.EndOfStream)
            {
                string line = inputStream.ReadLine();
                handler(line, model);
            }
        }

        private async Task LoadPolicyDataAsync(Model model, Helper.LoadPolicyLineHandler<string, Model> handler, StreamReader inputStream)
        {
            while (!inputStream.EndOfStream)
            {
                string line = await inputStream.ReadLineAsync();
                handler(line, model);
            }
        }

        private IList<string> ConvertToPolicyStrings(Model model)
        {
            var policy = new List<string>();
            policy.AddRange(GetModelPolicy(model, PermConstants.DefaultPolicyType));
            if (model.Model.ContainsKey(PermConstants.Section.RoleSection))
            {
                policy.AddRange(GetModelPolicy(model, PermConstants.Section.RoleSection));
            }
            return policy;
        }

        private void SavePolicyFile(string text)
        {
            File.WriteAllText(filePath, text, Encoding.UTF8);
        }

        private async Task SavePolicyFileAsync(string text)
        {
            text = text ?? string.Empty;
            var content = Encoding.UTF8.GetBytes(text);
            using (var fs = new FileStream(
                   filePath, FileMode.Create, FileAccess.Write,
                   FileShare.None, bufferSize: 4096, useAsync: true))
            {
                await fs.WriteAsync(content, 0, content.Length);
            }
        }

        public void AddPolicy(string sec, string ptype, IList<string> rule)
        {

        }

        public Task AddPolicyAsync(string sec, string ptype, IList<string> rule)
        {
            throw new NotImplementedException();
        }

        public void AddPolicies(string sec, string ptype, IEnumerable<IList<string>> rules)
        {
            throw new NotImplementedException();
        }

        public Task AddPoliciesAsync(string sec, string ptype, IEnumerable<IList<string>> rules)
        {
            throw new NotImplementedException();
        }

        public void RemovePolicy(string sec, string ptype, IList<string> rule)
        {

        }

        public Task RemovePolicyAsync(string sec, string ptype, IList<string> rule)
        {
            throw new NotImplementedException();
        }

        public void RemovePolicies(string sec, string ptype, IEnumerable<IList<string>> rules)
        {
            throw new NotImplementedException();
        }

        public Task RemovePoliciesAsync(string sec, string ptype, IEnumerable<IList<string>> rules)
        {
            throw new NotImplementedException();
        }

        public void RemoveFilteredPolicy(string sec, string ptype, int fieldIndex, params string[] fieldValues)
        {
            throw new NotImplementedException();
        }

        public Task RemoveFilteredPolicyAsync(string sec, string ptype, int fieldIndex, params string[] fieldValues)
        {
            throw new NotImplementedException();
        }
    }
}