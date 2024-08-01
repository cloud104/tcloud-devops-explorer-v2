using Accessibility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace TCLoudExplorer
{
    public class ListViewCache
    {
        // Regex para verificação do padrão de pasta \\
        private static bool VerifyStringFormat(string input)
        {
            // Define the regex pattern
            string pattern = @"^.*\\.*$";

            // Create a Regex object
            Regex regex = new Regex(pattern);

            // Perform the match
            Match match = regex.Match(input);

            // Return true if the match is successful, false otherwise
            return match.Success;
        }

        /*
         * Armazena um cache do _treeView com o conteúdo da pasta raiz do usuario para melhorar a performance
         * 
         * string: key = fullPath. Ex.: c:\ETC\PASTA1\PASTA_SUB\ é uma string única que identifica a pasta
         * List<ListViewItem>: value = lista de itens desta pasta: arquivos e pastas
         */
        private Dictionary<string, List<ListViewItem>> _treeCache;

        /*
         * Construtor
         */
        public ListViewCache()         {
            _treeCache = new Dictionary<string, List<ListViewItem>>();
        }

        /*
         * Remove toda uma lista de uma árvore dado uma key
         */
        public void RemoveAll(string fullPath)
        {
            if (!_treeCache.ContainsKey(fullPath.ToLower().Trim()))
                return;

            _treeCache.Remove(fullPath.ToLower().Trim());
        }

        /*
         * Remove um item de uma lista do cache se ele fizer parte do diretorio cache
         */
        public void Remove(string fullPath, string item)
        {
            if (!_treeCache.ContainsKey(fullPath.ToLower().Trim()))
                return;

            List<ListViewItem> _auxItems = _treeCache[fullPath];
            List<ListViewItem> _auxToRemove = new List<ListViewItem>();

            Parallel.ForEach(_auxItems, (_auxItem) =>
            //foreach (ListViewItem _auxItem in _auxItems)
            {
                if (_auxItem.Name.ToLower().Trim().Equals(item.ToLower().Trim()))
                {
                    _auxToRemove.Add(_auxItem);
                }
            });
            _auxItems.RemoveAll(x => _auxToRemove.Contains(x));
        }
        /**
         * Atualiza um item de uma lista dada uma key e item
         */
        public void Update(string fullPath, ListViewItem item)
        {
            if (!_treeCache.ContainsKey(fullPath.ToLower().Trim()))
                return;

            List<ListViewItem> items = _treeCache[fullPath.ToLower().Trim()];

            Parallel.ForEach(items, (item) =>
            {
                if (item.Text.ToLower().Trim().Equals(item.Text.ToLower().Trim()))
                {
                    items.Remove(item);
                    items.Add(item);
                }
            });
        }

        /*
         * Atualiza/Adiciona uma nova lista dada uma key
         */
        public void UpdateAll(string fullPath, List<ListViewItem> items)
        {
            if (_treeCache.ContainsKey(fullPath.ToLower().Trim()))
                _treeCache.Remove(fullPath.ToLower().Trim());

            _treeCache.Add(fullPath.ToLower().Trim(), items);
        }

        public List<ListViewItem> Get(string fullPath)
        {
            return _treeCache[fullPath];
        }

        /* 
         * Verifica se a pasta informada está no cache
         */
        public bool IsCached(string fullPath)
        {
            if (_treeCache.ContainsKey(fullPath.ToLower().Trim().Trim()))
            {
                return _treeCache[fullPath.ToLower().Trim()].Count > 0;
            }
            else
            {
                return false;
            }
        }
    }
}
