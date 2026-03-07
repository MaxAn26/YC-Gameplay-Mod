using MelonLoader;
using MelonLoader.Preferences;
using MelonLoader.Utils;

namespace BaseMod.Core;
public class PluginConfig {
    private readonly Dictionary<string, MelonPreferences_Category> _categories = new();
    private readonly string _filename;

    public PluginConfig(string filename) {
        _filename = filename;
    }

    public MelonPreferences_Entry<T> Entry<T>(string categoryName, string entryName, T defaultValue, string description, AcceptableValue validator = null) {
        if (!_categories.TryGetValue(categoryName, out MelonPreferences_Category category)){
            category = MelonPreferences.CreateCategory(categoryName);
            category.SetFilePath(Path.Combine(MelonEnvironment.UserDataDirectory, _filename));
            category.LoadFromFile();
            _categories.Add(categoryName, category);
        }

        if (validator is not null)
            description = $"{description} ({validator.AdditionalMessage})";

        return category.CreateEntry(entryName, defaultValue, description: description, validator: validator);
    }

    public void Save() {
        foreach ( var category in _categories.Values ) {
            category.SaveToFile();
        }
    }

    public abstract class AcceptableValue : ValueValidator {
        public abstract string AdditionalMessage { get; }
    }

    public class AcceptableValueList<T> : AcceptableValue {
        private readonly IList<T> _values;

        public override string AdditionalMessage => $"Acceptable Values {string.Join(", ", _values)}";

        public AcceptableValueList(IList<T> values) {
            _values = new List<T>(values);
        }

        public override object EnsureValid(object value) {
            if (value is not T castedValue || !_values.Contains(castedValue))
                return default;

            return castedValue;
        }

        public override bool IsValid(object value) {
            return value is T castedValue && _values.Contains(castedValue);
        }
    }

    public class AcceptableValueRange<T> : AcceptableValue where T : IComparable {
        private readonly T _minValue;
        private readonly T _maxValue;

        public override string AdditionalMessage => $"Acceptable Range from {_minValue} to {_maxValue}";

        public AcceptableValueRange(T minValue, T maxValue) { 
            _minValue = minValue;
            _maxValue = maxValue;
        }

        public override object EnsureValid(object value) {
            if (value is not T castedValue)
                return _minValue;
            else if (_maxValue.CompareTo(castedValue) < 0)
                return _maxValue;
            else if (_minValue.CompareTo(castedValue) > 0)
                return _minValue;

                return castedValue;
        }

        public override bool IsValid(object value) {
            return value is T castedValue && _minValue.CompareTo(castedValue) <= 0 && _maxValue.CompareTo(castedValue) >= 0;
        }
    }
}