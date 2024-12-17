using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Utils {
    public enum Unit {
        None = 0,
        Kilo = 1,
        Mega = 2,
        Giga = 3
    }

    [JsonConverter(typeof(DataSizeJsonConverter))]
    public struct DataSize : IEquatable<DataSize> {
        public Unit Unit;
        public double Value;

        public DataSize() {
            Unit = Unit.None;
            Value = 0.0;
        }

        public DataSize(double value, Unit unit) {
            bool isNeg = (value < 0);
            value = Math.Abs(value);
            while (value >= 0.0 && value < 1.0 && (int)unit > (int)Unit.None)
            {
                value *= 1000;
                unit = (Unit)((int)unit - 1);
            }
            while (value >= 1000.0 && (int)unit < (int)Unit.Giga)
            {
                value /= 1000;
                unit = (Unit)((int)unit + 1);
            }
            if (isNeg)
            {
                value = -value;
            }

            if (unit < Unit.None || unit > Unit.Giga)
            {
                throw new ArgumentOutOfRangeException($"Value is either too big or small: {value} @ unit={(int)unit}");
            }

            this.Unit = unit;
            this.Value = value;
        }

        // Returns the size of the value represented in gigabytes
        public double AsGB() {
            switch(this.Unit)
            {
                case Unit.None: return this.Value / 1000000000.0;
                case Unit.Kilo: return this.Value / 1000000.0;
                case Unit.Mega: return this.Value / 1000.0;
                case Unit.Giga: return this.Value;
                default: throw new Exception("Unknown unit.");
            }
        }

        public bool Near(DataSize other, DataSize maxDelta) {
            DataSize delta;
            if (this >= other)
            {
                delta = this - other;
            } else
            {
                delta = other - this;
            }
            return delta <= maxDelta;
        }

        public bool Near(DataSize other, double percent = 0.01) {
            DataSize delta;
            DataSize maxDelta;
            if (this >= other)
            {
                delta = this - other;
                maxDelta = this * percent;
            } else
            {
                delta = other - this;
                maxDelta = other * percent;
            }
            return delta <= maxDelta;
        }

        private string GetUnitString() {
            switch(this.Unit)
            {
                case Unit.None: return "B";
                case Unit.Kilo: return "KB";
                case Unit.Mega: return "MB";
                case Unit.Giga: return "GB";
                default:
                    throw new Exception("Unknown unit type.");
            }
        }

        public override string ToString() {
            return $"{this.Value:0.00} {GetUnitString()}";
        }

        public static DataSize Parse(string str) {
            str = str.Trim();
            string[] parts = str.Split();
            if (parts.Length != 2)
            {
                throw new FormatException("Bad DataSize format");
            }

            double value = double.Parse(parts[0]);
            switch(parts[1])
            {
                case "GB": return new(value, Unit.Giga);
                case "MB": return new(value, Unit.Mega);
                case "KB": return new(value, Unit.Kilo);
                case "B": return new(value, Unit.None);
                default: throw new FormatException("Unknown byte size: " + parts[1]);
            }
        }

        public bool Equals(DataSize other) {
            return this.Near(other);
        }

        public static DataSize operator +(DataSize left, DataSize right) => new(left.AsGB() + right.AsGB(), Unit.Giga);
        public static DataSize operator -(DataSize left, DataSize right) => new(left.AsGB() - right.AsGB(), Unit.Giga);
        public static DataSize operator *(DataSize left, double right) => new(left.AsGB() * right, Unit.Giga);
        public static DataSize operator *(double left, DataSize right) => new(left * right.AsGB(), Unit.Giga);
        public static DataSize operator /(DataSize left, double right) => new(left.AsGB() / right, Unit.Giga);
        public static DataSize operator /(double left, DataSize right) => new(left / right.AsGB(), Unit.Giga);
        public static bool operator ==(DataSize left, DataSize right) => (left.Unit == right.Unit) && (left.Value == right.Value);
        public static bool operator !=(DataSize left, DataSize right) => !(left == right);
        public static bool operator <(DataSize left, DataSize right) {
            if (left.Unit == right.Unit)
            {
                return left.Value < right.Value;
            }else
            {
                return left.Unit < right.Unit;
            }
        }
        public static bool operator >(DataSize left, DataSize right) {
            if (left.Unit == right.Unit)
            {
                return left.Value > right.Value;
            } else
            {
                return left.Unit > right.Unit;
            }
        }
        public static bool operator <=(DataSize left, DataSize right) {
            if (left.Unit == right.Unit)
            {
                return left.Value <= right.Value;
            } else
            {
                return left.Unit < right.Unit;
            }
        }
        public static bool operator >=(DataSize left, DataSize right) {
            if (left.Unit == right.Unit)
            {
                return left.Value >= right.Value;
            } else
            {
                return left.Unit > right.Unit;
            }
        }

        public override bool Equals(object? obj) {
            if (obj == null)
            {
                // I assume this != null, so this can't be true
                return false;
            } else if (obj is DataSize other)
            {
                return this == other;
            } else
            {
                return false;
            }
        }

        public override int GetHashCode() {
            return this.Value.GetHashCode();
        }

        public static DataSize? FromFile(string path)
        {
            try
            {
                return new DataSize(new FileInfo(path).Length, Unit.None);
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        private class DataSizeJsonConverter : JsonConverter<DataSize>
        {
            public override DataSize Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            {
                string? str = reader.GetString();
                if (str == null) throw new JsonException("Failed to parse null string.");
                else return DataSize.Parse(str);
            }

            public override void Write(Utf8JsonWriter writer, DataSize value, JsonSerializerOptions options)
            {
                writer.WriteStringValue($"{value.Value} {value.GetUnitString()}");
            }
        }
    }
}
