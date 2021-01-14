﻿using System;
using System.Collections;
using System.Globalization;
using System.IO;

namespace DevilDaggersWebsite.Tasks.Cron
{
	/// <summary>
	/// Represents a single crontab field.
	/// </summary>
	[Serializable]
	public sealed class CrontabField
	{
		private readonly BitArray _bits;
		private readonly CrontabFieldImpl _impl;
		private int _maxValueSet;
		private int _minValueSet;

		private CrontabField(CrontabFieldImpl impl, string expression)
		{
			_impl = impl ?? throw new ArgumentNullException(nameof(impl));
			_bits = new BitArray(impl.ValueCount);

			_bits.SetAll(false);
			_minValueSet = int.MaxValue;
			_maxValueSet = -1;

			_impl.Parse(expression, Accumulate);
		}

		#region ICrontabField Members

		/// <summary>
		/// Gets the first value of the field or -1.
		/// </summary>
		public int GetFirst() => _minValueSet < int.MaxValue ? _minValueSet : -1;

		/// <summary>
		/// Gets the next value of the field that occurs after the given
		/// start value or -1 if there is no next value available.
		/// </summary>
		public int Next(int start)
		{
			if (start < _minValueSet)
				return _minValueSet;

			int startIndex = ValueToIndex(start);
			int lastIndex = ValueToIndex(_maxValueSet);

			for (int i = startIndex; i <= lastIndex; i++)
			{
				if (_bits[i])
					return IndexToValue(i);
			}

			return -1;
		}

		/// <summary>
		/// Determines if the given value occurs in the field.
		/// </summary>
		public bool Contains(int value) => _bits[ValueToIndex(value)];

		#endregion ICrontabField Members

		/// <summary>
		/// Parses a crontab field expression given its kind.
		/// </summary>
		public static CrontabField Parse(CrontabFieldKind kind, string expression) => new CrontabField(CrontabFieldImpl.FromKind(kind), expression);

		/// <summary>
		/// Parses a crontab field expression representing minutes.
		/// </summary>
		public static CrontabField Minutes(string expression) => new CrontabField(CrontabFieldImpl.Minute, expression);

		/// <summary>
		/// Parses a crontab field expression representing hours.
		/// </summary>
		public static CrontabField Hours(string expression) => new CrontabField(CrontabFieldImpl.Hour, expression);

		/// <summary>
		/// Parses a crontab field expression representing days in any given month.
		/// </summary>
		public static CrontabField Days(string expression) => new CrontabField(CrontabFieldImpl.Day, expression);

		/// <summary>
		/// Parses a crontab field expression representing months.
		/// </summary>
		public static CrontabField Months(string expression) => new CrontabField(CrontabFieldImpl.Month, expression);

		/// <summary>
		/// Parses a crontab field expression representing days of a week.
		/// </summary>
		public static CrontabField DaysOfWeek(string expression) => new CrontabField(CrontabFieldImpl.DayOfWeek, expression);

		private int IndexToValue(int index) => index + _impl.MinValue;

		private int ValueToIndex(int value) => value - _impl.MinValue;

		/// <summary>
		/// Accumulates the given range (start to end) and interval of values
		/// into the current set of the field.
		/// </summary>
		/// <remarks>
		/// To set the entire range of values representable by the field,
		/// set <param name="start" /> and <param name="end" /> to -1 and
		/// <param name="interval" /> to 1.
		/// </remarks>
		private void Accumulate(int start, int end, int interval)
		{
			int minValue = _impl.MinValue;
			int maxValue = _impl.MaxValue;

			if (start == end)
			{
				if (start < 0)
				{
					// We're setting the entire range of values.
					if (interval <= 1)
					{
						_minValueSet = minValue;
						_maxValueSet = maxValue;
						_bits.SetAll(true);
						return;
					}

					start = minValue;
					end = maxValue;
				}
				else
				{
					// We're only setting a single value - check that it is in range.
					if (start < minValue)
					{
						throw new FormatException($"'{start} is lower than the minimum allowable value for this field. Value must be between {_impl.MinValue} and {_impl.MaxValue} (all inclusive).");
					}

					if (start > maxValue)
					{
						throw new FormatException($"'{end} is higher than the maximum allowable value for this field. Value must be between {_impl.MinValue} and {_impl.MaxValue} (all inclusive).");
					}
				}
			}
			else
			{
				// For ranges, if the start is bigger than the end value then
				// swap them over.
				if (start > end)
				{
					end ^= start;
					start ^= end;
					end ^= start;
				}

				if (start < 0)
				{
					start = minValue;
				}
				else if (start < minValue)
				{
					throw new FormatException($"'{start} is lower than the minimum allowable value for this field. Value must be between {_impl.MinValue} and {_impl.MaxValue} (all inclusive).");
				}

				if (end < 0)
				{
					end = maxValue;
				}
				else if (end > maxValue)
				{
					throw new FormatException($"'{end} is higher than the maximum allowable value for this field. Value must be between {_impl.MinValue} and {_impl.MaxValue} (all inclusive).");
				}
			}

			if (interval < 1)
				interval = 1;

			int i;

			// Populate the _bits table by setting all the bits corresponding to
			// the valid field values.
			for (i = start - minValue; i <= end - minValue; i += interval)
				_bits[i] = true;

			// Make sure we remember the minimum value set so far Keep track of
			// the highest and lowest values that have been added to this field
			// so far.
			if (_minValueSet > start)
				_minValueSet = start;

			i += minValue - interval;

			if (_maxValueSet < i)
				_maxValueSet = i;
		}

		public override string ToString() => ToString(null);

		public string ToString(string? format)
		{
			using StringWriter writer = new StringWriter(CultureInfo.InvariantCulture);
			switch (format)
			{
				case "G":
				case null:
					Format(writer, true);
					break;
				case "N":
					Format(writer);
					break;
				default:
					throw new FormatException();
			}

			return writer.ToString();
		}

		public void Format(TextWriter writer)
		{
			Format(writer, false);
		}

		public void Format(TextWriter writer, bool noNames)
		{
			_impl.Format(this, writer, noNames);
		}
	}
}
