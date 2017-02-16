﻿using System.Linq;

namespace Arsenalcn.Core
{
    public class Criteria : IPager
    {
        public Criteria()
        {
            GetPageSize();
        }

        public Criteria(object para, short pagesize = 0)
        {
            Parameters = para;

            PagingSize = pagesize;
        }

        public object Parameters { get; set; }
        public string WhereClause { get; set; }
        public string OrderClause { get; set; }

        public short PagingSize { get; set; }
        public int CurrentPage { get; set; }

        public int MaxPage { get; private set; }
        public int TotalCount { get; private set; }

        public void GetPageSize()
        {
            if (PagingSize <= 0)
            {
                PagingSize = 10;
            }
        }

        public string GetWhereClause()
        {
            var propties = Parameters?.GetType().GetProperties();

            if (Parameters == null && string.IsNullOrEmpty(WhereClause)) { return null; }

            var strPara = string.Empty;

            if (propties != null && propties.Any())
            {
                var arrWhere = new string[propties.Length];

                var i = 0;

                foreach (var p in propties)
                {
                    arrWhere[i] = $" {p.Name} = @{p.Name}";
                }

                strPara = string.Join(" AND ", arrWhere);
            }

            return WhereClause ?? string.Empty + strPara;
        }

        public void SetTotalCount(int value)
        {
            TotalCount = value;

            GetPageSize();

            MaxPage = TotalCount / PagingSize;

            if (CurrentPage > MaxPage)
            {
                CurrentPage = MaxPage;
            }
        }
    }
}