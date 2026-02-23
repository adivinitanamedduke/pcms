namespace Domain.Utilities;

public class ProductSearchEngine<T> where T : class
{
    private readonly List<T> _data;
    private readonly Dictionary<string, double> _weightedFields;

    public ProductSearchEngine(IEnumerable<T> data, Dictionary<string, double> weightedFields)
    {
        _data = data.ToList();
        _weightedFields = weightedFields;
    }

    public IEnumerable<T> Search(string query, double threshold = 0.7)
    {
        if (string.IsNullOrWhiteSpace(query)) return _data;

        return _data
            .Select(item => new { Item = item, Score = CalculateScore(item, query.ToLower()) })
            .Where(x => x.Score >= threshold)
            .OrderByDescending(x => x.Score)
            .Select(x => x.Item);
    }

    private double CalculateScore(T item, string query)
    {
        double totalScore = 0;
        var properties = typeof(T).GetProperties();

        foreach (var field in _weightedFields)
        {
            var prop = properties.FirstOrDefault(p => p.Name.Equals(field.Key, StringComparison.OrdinalIgnoreCase));
            var value = prop?.GetValue(item)?.ToString()?.ToLower();

            if (string.IsNullOrEmpty(value)) continue;

            double similarity = 1 - (double)ComputeDistance(value, query) / Math.Max(value.Length, query.Length);

            // Weight the similarity, Name matches are worth more than Description
            totalScore += similarity * field.Value;
        }

        return totalScore;
    }

    // Levenshtein Distance Algorithm for Fuzzy Matching
    private static int ComputeDistance(string s, string t)
    {
        int n = s.Length;
        int m = t.Length;
        int[,] d = new int[n + 1, m + 1];

        if (n == 0) return m;
        if (m == 0) return n;

        for (int i = 0; i <= n; d[i, 0] = i++) ;
        for (int j = 0; j <= m; d[0, j] = j++) ;

        for (int i = 1; i <= n; i++)
        {
            for (int j = 1; j <= m; j++)
            {
                int cost = (t[j - 1] == s[i - 1]) ? 0 : 1;
                d[i, j] = Math.Min(Math.Min(d[i - 1, j] + 1, d[i, j - 1] + 1), d[i - 1, j - 1] + cost);
            }
        }
        return d[n, m];
    }
}
