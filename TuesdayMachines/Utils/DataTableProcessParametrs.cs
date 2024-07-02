using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using MongoDB.Driver;
using System.Linq.Expressions;
using System.Reflection;
using System.Collections;

namespace TuesdayMachines.Utils
{
    public class DataTableProcessParametrs
    {
        public class SearchData
        {
            public bool Regex { get; set; }
            public string Value { get; set; }
        }
        public class ColumndData
        {
            public string Data { get; set; }
            public SearchData Search { get; set; }
        }

        public class OrderData
        {
            public int Column { get; set; }
            public string Dir { get; set; }
        };
        public ColumndData[] Columns { get; set; }
        public int Draw { get; set; }
        public int Start { get; set; }
        public int Length { get; set; }
        public OrderData[] Order { get; set; }
        public SearchData Search { get; set; }
    };

    public enum DatatableFileInfoSerach
    {
        Eq,
        Regex,
        Text,
        Range
    };

    internal class DatatableFiledInfo
    {
        public string Name { get; set; }
        public BsonType Type { get; set; }
        public bool IsEnumerable { get; set; }
        public DatatableFileInfoSerach SearchType { get; set; }
    }

    public class DatatablesPagingDefinition<T>
    {
        private DataTableProcessParametrs _parametrs;
        private FindOptions<T> _findOptions;
        private FilterDefinition<T> _filter;

        private List<DatatableFiledInfo> _allowedFilters;
        private List<DatatableFiledInfo> _globalSearchFields;

        public DatatablesPagingDefinition(DataTableProcessParametrs parametrs)
        {
            _parametrs = parametrs;
            _allowedFilters = new List<DatatableFiledInfo>();
            _globalSearchFields = new List<DatatableFiledInfo>();
            _findOptions = new FindOptions<T>() { Skip = parametrs.Start, Limit = parametrs.Length };
            _filter = Builders<T>.Filter.Empty;
        }

        public FindOptions<T> GetOptions()
        {
            return _findOptions;
        }

        public FilterDefinition<T> GetFilter()
        {
            return _filter;
        }

        public DatatablesPagingDefinition<T> IgnoreCaseSerach()
        {
            _findOptions.Collation = new Collation("en", strength: CollationStrength.Secondary);
            return this;
        }

        public DatatablesPagingDefinition<T> ApplySort()
        {
            SortDefinition<T> sort = null;

            foreach (var order in _parametrs.Order)
            {
                var columnName = GetColumnName(_parametrs.Columns[order.Column].Data);

                if (order.Dir == "asc")
                    sort = sort == null ? Builders<T>.Sort.Ascending(columnName) : sort.Ascending(columnName);
                else
                    sort = sort == null ? Builders<T>.Sort.Descending(columnName) : sort.Descending(columnName);
            }

            if (sort != null)
                _findOptions.Sort = sort;
            return this;
        }

        public DatatablesPagingDefinition<T> AddGlobalFilterField(Expression<Func<T, object>> field, DatatableFileInfoSerach searchType)
        {
            var result = GetFieldInfo(field);
            result.SearchType = searchType;
            _globalSearchFields.Add(result);
            return this;
        }

        public DatatablesPagingDefinition<T> AllowFilterFor(Expression<Func<T, object>> field, DatatableFileInfoSerach searchType)
        {
            var result = GetFieldInfo(field);
            result.SearchType = searchType;
            _allowedFilters.Add(result);
            return this;
        }

        public DatatablesPagingDefinition<T> ApplyColumnFilter()
        {
            FilterDefinition<T> columnFilter = null;

            foreach (var column in _parametrs.Columns)
            {
                if (column.Search == null || string.IsNullOrEmpty(column.Search.Value)) continue;
                var name = GetColumnName(column.Data);
                var fieldInfo = _allowedFilters.FirstOrDefault(x => x.Name == name);
                if (fieldInfo == null) continue;

                var filter = CreateFilter(fieldInfo, column.Search.Value);
                if (columnFilter == null)
                    columnFilter = filter;
                else
                    columnFilter &= filter;
            }

            ApplyToFilterChain(columnFilter);
            return this;
        }

        public DatatablesPagingDefinition<T> ApplyGlobalFilter()
        {
            FilterDefinition<T> globalFilter = null;
            if (_parametrs.Search != null && !string.IsNullOrEmpty(_parametrs.Search.Value))
            {
                foreach (var serach in _globalSearchFields)
                {
                    var filter = CreateFilter(serach, _parametrs.Search.Value);
                    if (globalFilter == null)
                        globalFilter = filter;
                    else
                        globalFilter |= filter;
                }
            }

            ApplyToFilterChain(globalFilter);

            return this;
        }

        public DatatablesPagingDefinition<T> AddStaticFilter(Expression<Func<T, object>> field, object value)
        {
            ApplyToFilterChain(Builders<T>.Filter.Eq(field, value));
            return this;
        }

        public async Task<(List<T>, long, long)> Execute(IMongoCollection<T> collection)
        {
            var totalRecords = await collection.EstimatedDocumentCountAsync();
            var filteredRecords = totalRecords;
            if (_filter != Builders<T>.Filter.Empty)
                filteredRecords = await collection.CountDocumentsAsync(_filter);

            var data = await (await collection.FindAsync(_filter, _findOptions)).ToListAsync();

            return (data, filteredRecords, totalRecords);
        }

        public DatatablesPagingDefinition<T> SortDescendingById()
        {
            _findOptions.Sort = _findOptions.Sort == null ? Builders<T>.Sort.Descending("_id") : _findOptions.Sort.Descending("_id");

            return this;
        }

        public DatatablesPagingDefinition<T> SortDescendingBy(Expression<Func<T, object>> field)
        {
            var fieldInfo = GetFieldInfo(field);

            _findOptions.Sort = _findOptions.Sort == null ? Builders<T>.Sort.Descending(fieldInfo.Name) : _findOptions.Sort.Descending(fieldInfo.Name);

            return this;
        }

        private string GetColumnName(string dataName)
        {
            if (char.IsLower(dataName[0]))
                dataName = char.ToUpper(dataName[0]) + dataName.Substring(1);

            return dataName;
        }

        private void ApplyToFilterChain(FilterDefinition<T> filter)
        {
            if (filter != null)
            {
                if (_filter == null)
                    _filter = filter;
                else
                    _filter &= filter;
            }
        }

        private FilterDefinition<T> CreateFilter(DatatableFiledInfo info, string value)
        {
            if (info == null || string.IsNullOrEmpty(value))
                return null;

            if (info.Type == BsonType.String)
            {
                if (info.SearchType == DatatableFileInfoSerach.Regex)
                    return Builders<T>.Filter.Regex(info.Name, new BsonRegularExpression("/" + value + "/i"));
                return Builders<T>.Filter.Eq(info.Name, value);
            }
            else if (info.Type == BsonType.ObjectId)
            {
                if (ObjectId.TryParse(value, out var objId))
                    return Builders<T>.Filter.Eq(info.Name, objId);
            }
            else if (info.Type == BsonType.Boolean)
            {
                if (bool.TryParse(value, out var valueBool))
                    return Builders<T>.Filter.Eq(info.Name, valueBool);
            }
            else if (info.Type == BsonType.Int64
                || info.Type == BsonType.Int32)
            {
                var between = value.IndexOf('-');
                if (between == -1)
                    return null;

                var from = value.Substring(0, between);
                var to = value.Substring(between + 1);

                if (info.Type == BsonType.Int64)
                {
                    if (long.TryParse(from, out var fromValue)
                        && long.TryParse(to, out var toValue))
                    {
                        return Builders<T>.Filter.And(Builders<T>.Filter.Gte(info.Name, fromValue), Builders<T>.Filter.Lte(info.Name, toValue));
                    }
                }
                else if (info.Type == BsonType.Int32)
                {
                    if (int.TryParse(from, out var fromValue)
                        && int.TryParse(to, out var toValue))
                    {
                        return Builders<T>.Filter.And(Builders<T>.Filter.Gte(info.Name, fromValue), Builders<T>.Filter.Lte(info.Name, toValue));
                    }
                }
            }

            return null;
        }

        private DatatableFiledInfo GetFieldInfo(Expression<Func<T, object>> field)
        {
            var result = new DatatableFiledInfo() { Type = BsonType.String, SearchType = DatatableFileInfoSerach.Eq };
            MemberInfo member = null;

            if (field.Body is MemberExpression)
            {
                member = ((MemberExpression)field.Body).Member;
            }
            else if (field.Body is UnaryExpression)
            {
                var unaryOperand = ((UnaryExpression)field.Body).Operand;
                if (unaryOperand is MemberExpression)
                {
                    member = ((MemberExpression)unaryOperand).Member;
                }
            }

            result.Name = member.Name;

            Type fieldType = null;

            if (member.MemberType == MemberTypes.Property)
            {
                fieldType = ((PropertyInfo)member).PropertyType;
            }
            else if (member.MemberType == MemberTypes.Field)
            {
                fieldType = ((FieldInfo)member).FieldType;
            }

            if (fieldType != null)
            {
                result.IsEnumerable = fieldType.GetInterface(nameof(IEnumerable)) != null;

                if (fieldType == typeof(bool)
                    || fieldType == typeof(Boolean))
                {
                    result.Type = BsonType.Boolean;
                }

                if (fieldType == typeof(long)
                    || fieldType == typeof(Int64))
                {
                    result.Type = BsonType.Int64;
                }
            }

            var attributes = member.CustomAttributes;
            foreach (var attribute in attributes)
            {
                if (attribute.AttributeType == typeof(BsonRepresentationAttribute))
                {
                    result.Type = (BsonType)attribute.ConstructorArguments.FirstOrDefault().Value;
                    break;
                }
            }

            return result;
        }
    }

    public class Datatables
    {
        public static DatatablesPagingDefinition<T> StartPaging<T>(DataTableProcessParametrs parametrs)
        {
            return new DatatablesPagingDefinition<T>(parametrs);
        }
    }
}
