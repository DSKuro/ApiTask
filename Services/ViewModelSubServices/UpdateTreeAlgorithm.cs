using ApiTask.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ApiTask.Services.ViewModelSubServices
{
    public class UpdateTreeAlgorithm
    {
        private static readonly string NonCategorizedTitle = "Без категории";
        private SortingTreeMemento SortingTreeState;
        private List<string> CodesData;
        private List<List<string>> Params;
        SmartCollection<Codes> Categories;

        public UpdateTreeAlgorithm(SortingTreeMemento sortingTreeState, List<string> codesData, List<List<string>> codesParams,
            SmartCollection<Codes> categories)
        {
            SortingTreeState = sortingTreeState;
            CodesData = codesData;
            Params = codesParams;
            Categories = categories;
        }

        public void UpdateCategories()
        {
            (List<string> nonCategorized, List<string> allCategorized,
                Dictionary<string, List<string>> partialCategorized) = InitializeCategories();
            DefineCategories(nonCategorized, allCategorized, partialCategorized);
            UpdateCategoriesInBinding(nonCategorized, allCategorized, partialCategorized);
        }

        private (List<string>, List<string>, Dictionary<string, List<string>>) InitializeCategories()
        {
            List<string> nonCategorized = new List<string>();
            List<string> allCategorized = new List<string>();
            Dictionary<string, List<string>> partialCategorized = new Dictionary<string, List<string>>();
            foreach (string param in SortingTreeState.EnabledParameters)
            {
                partialCategorized.Add(param, new List<string>());
            }
            return (nonCategorized, allCategorized, partialCategorized);
        }

        private void DefineCategories(List<string> nonCategorized, List<string> allCategorized,
            Dictionary<string, List<string>> partialCategorized)
        {
            List<int> changed = new List<int>(SortingTreeState.EnabledParameters.Count);
            for (int i = 0; i < Params.Count; i++)
            {
                
                for (int j = 0; j < SortingTreeState.EnabledParameters.Count; j++)
                {

                    if (!Params[i].Contains(SortingTreeState.EnabledParameters[j]))
                    {
                        changed.Add(j);
                    }
                }
                DefineCategoriesImpl(nonCategorized, allCategorized, partialCategorized, changed, i);
                changed.Clear();
            }
        }

        private void DefineCategoriesImpl(List<string> nonCategorized, List<string> allCategorized,
            Dictionary<string, List<string>> partialCategorized, List<int> param, int i)
        {
            if (param.Count == SortingTreeState.EnabledParameters.Count)
            {
                nonCategorized.Add(CodesData[i]);
            }
            else if (param.Count == 0)
            {
                allCategorized.Add(CodesData[i]);
            }
            else
            {
                foreach (int index in param)
                {
                    partialCategorized[SortingTreeState.EnabledParameters[index]].Add(CodesData[i]);
                }
            }
        }

        private void UpdateCategoriesInBinding(List<string> nonCategorized, List<string> allCategorized,
            Dictionary<string, List<string>> partialCategorized)
        {
            if (nonCategorized.Count != 0 && nonCategorized.Count != CodesData.Count)
            {
                CreateCategory(nonCategorized, NonCategorizedTitle);
            }

            IEnumerable<KeyValuePair<string, List<string>>> result = partialCategorized
                .Where(pair => pair.Value.Count != 0);
            if (result.Count() != 0)
            {
                foreach (KeyValuePair<string, List<string>> pair in result)
                {
                    CreateCategory(pair.Value, $"Без параметра: {pair.Key}");
                }
            }

            if (allCategorized.Count != 0)
            {
                CreateCategory(allCategorized, string.Join(", ", SortingTreeState.EnabledParameters.ToArray()));
            }

            if (partialCategorized.Count == 0 && allCategorized.Count == 0)
            {
                FillAllCodes();
            } 
        }

        private void CreateCategory(List<string> content, string title)
        {
            CodeCategory category = new CodeCategory(title);
            foreach (string code in content)
            {
                category.Codes.Add(new Codes(code));
            }
            Categories.Add(category);
        }

        private void FillAllCodes()
        {
            List<Codes> codes = new List<Codes>();
            foreach (string code in CodesData)
            {
                codes.Add(new Codes(code));
            }
            Categories.AddRange(codes);
        }
    }
}
