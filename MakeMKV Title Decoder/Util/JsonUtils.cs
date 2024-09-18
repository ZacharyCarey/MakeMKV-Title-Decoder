using JsonSerializable;
using LibVLCSharp.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MakeMKV_Title_Decoder
{
	public static partial class Utils
	{

		// TODO rename or use better json library
		public static void LoadJson(this JsonData input, string key, out string? result)
		{
			if (input == null) throw new ArgumentNullException("input");

			JsonData? data = ((JsonObject)input)[key];
			if (data == null)
			{
				result = null;
				return;
			}

			if (data is JsonString str)
			{
				result = str.Value;
			} else
			{
				Console.ForegroundColor = ConsoleColor.Yellow;
				Console.WriteLine("Failed to read JSON value.");
				Console.ResetColor();
				result = null;
				return;
			}
		}

		public static void LoadJson(this JsonData input, string key, out long? result)
		{
			if (input == null) throw new ArgumentNullException("input");

			JsonData? data = ((JsonObject)input)[key];
			if (data == null)
			{
				result = null;
				return;
			}

			if (data is JsonInteger integer)
			{
				result = integer.Value;
				return;
			} else if (data is JsonString str && str.Value == null)
			{
				result = null;
				return;
			} else
			{
				Console.ForegroundColor = ConsoleColor.Yellow;
				Console.WriteLine("Failed to read JSON value.");
				Console.ResetColor();
				result = null;
				return;
			}
		}

		public static void LoadJson(this JsonData input, string key, out bool? result)
		{
			if (input == null) throw new ArgumentNullException("input");

			JsonData? data = ((JsonObject)input)[key];
			if (data == null)
			{
				result = null;
				return;
			}

			if (data is JsonBool boolean)
			{
				result = boolean.Value;
				return;
			} else if (data is JsonString str && str.Value == null)
			{
				result = null;
				return;
			} else
			{
				Console.ForegroundColor = ConsoleColor.Yellow;
				Console.WriteLine("Failed to read JSON value.");
				Console.ResetColor();
				result = null;
				return;
			}
		}

		public static void LoadJson<T>(this JsonData input, string key, out T? result) where T : struct, Enum
		{
			if (input == null) throw new ArgumentNullException("input");

			JsonData? data = ((JsonObject)input)[key];
			if (data == null)
			{
				result = null;
				return;
			}

			if (data is JsonString str)
			{
				if (str.Value == null)
				{
					result = null;
					return;
				} else
				{
					object? temp;
					if (Enum.TryParse(typeof(T), str.Value, out temp))
					{
						result = (T)temp;
						return;
					}
				}
			}

			Console.ForegroundColor = ConsoleColor.Yellow;
			Console.WriteLine("Failed to read JSON value.");
			Console.ResetColor();
			result = null;
			return;
		}

		public static void LoadJsonClass<T>(this JsonData input, string key, out T? result) where T : class, IJsonSerializable, new()
		{
			if (input == null) throw new ArgumentNullException("input");

			JsonData? data = ((JsonObject)input)[key];
			if (data == null)
			{
				result = null;
				return;
			}

			result = new();
			result.LoadFromJson(data);
		}

		public static void LoadJsonStruct<T>(this JsonData input, string key, out T? result) where T : struct, IJsonSerializable
		{
			if (input == null) throw new ArgumentNullException("input");

			JsonData? data = ((JsonObject)input)[key];
			if (data == null)
			{
				result = null;
				return;
			}

			result = new();
			result.Value.LoadFromJson(data);
		}

		public static void LoadJson(this JsonData input, string key, out TimeSpan? result)
		{
			if (input == null) throw new ArgumentNullException("input");

			JsonData? data = ((JsonObject)input)[key];
			if (data == null)
			{
				result = null;
				return;
			}

			if (data is JsonString str)
			{
				if (str.Value == null)
				{
					result = null;
					return;
				}

				TimeSpan temp;
				if (TimeSpan.TryParse(str.Value, out temp))
				{
					result = temp;
					return;
				}
			}

			Console.ForegroundColor = ConsoleColor.Yellow;
			Console.WriteLine("Failed to read JSON value.");
			Console.ResetColor();
			result = null;
		}

		public static JsonData? SaveJson(string? input)
		{
			return new JsonString(input);
		}

		public static JsonData? SaveJson(long? input)
		{
			if (input.HasValue)
			{
				return new JsonInteger(input.Value);
			} else
			{
				return new JsonString(null);
			}
		}

		public static JsonData? SaveJson(bool? input)
		{
			if (input.HasValue)
			{
				return new JsonBool(input.Value);
			} else
			{
				return new JsonString(null);
			}
		}

		public static JsonData? SaveJson<T>(T? input) where T : struct, Enum
		{
			return new JsonString(input?.ToString());
		}

		public static JsonData? SaveJson(IJsonSerializable? input)
		{
			if (input != null)
			{
				return input.SaveToJson();
			} else
			{
				return new JsonString(null);
			}
		}

		public static JsonData? SaveJson(TimeSpan? input)
		{
			if (input.HasValue)
			{
				return new JsonString(input.ToString());
			} else
			{
				return new JsonString(null);
			}
		}
	}
}
