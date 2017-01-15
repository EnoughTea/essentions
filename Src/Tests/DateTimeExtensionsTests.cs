using System;
using NUnit.Framework;

namespace Essentions.Tests
{
    [TestFixture]
    public class DateTimeExtensionsTests
    {
        [Test]
        public void When_ToRfcFormat_is_used_with_local_date_and_hour_offset_Then_resulting_date_should_conform_to_format()
        {
            DateTime validLocalDate = new DateTime(2016, 11, 03, 21, 08, 59, DateTimeKind.Local);
            string rfc822 = validLocalDate.ToRfc822();
            Assert.That(rfc822, Is.Not.Null);
            Assert.That(rfc822, Is.Not.Empty);
            string[] rfc822Parts = rfc822.Split(" ");
            Assert.That(rfc822Parts.Length, Is.EqualTo(5));
            Assert.That(rfc822Parts[0] == "Mon," | rfc822Parts[0] == "Thu," || rfc822Parts[0] == "Wed,", Is.True);
            Assert.That(Convert.ToInt32(rfc822Parts[1]), Is.InRange(1, 31));
            Assert.That(rfc822Parts[2], Is.EqualTo("Nov"));
            Assert.That(rfc822Parts[3], Is.EqualTo("2016"));
            Assert.That(rfc822Parts[4].Length, Is.EqualTo(13));
            Assert.That(rfc822Parts[4].Contains("+") || rfc822Parts[4].Contains("-"), Is.True);
            Assert.That(rfc822Parts[4].Substring(rfc822Parts[4].Length - 4), Is.Not.EqualTo("0000"));
        }

        [Test]
        public void When_ToRfcFormat_is_used_with_UTC_date_and_hour_offset_Then_resulting_date_should_conform_to_format()
        {
            DateTime validUtcDate = new DateTime(2016, 11, 03, 21, 08, 59, DateTimeKind.Utc);
            string rfc822 = validUtcDate.ToRfc822();
            Assert.That(rfc822, Is.Not.Null);
            Assert.That(rfc822, Is.Not.Empty);
            string[] rfc822Parts = rfc822.Split(" ");
            Assert.That(rfc822Parts.Length, Is.EqualTo(5));
            Assert.That(rfc822Parts[0] == "Mon," | rfc822Parts[0] == "Thu," || rfc822Parts[0] == "Wed,", Is.True);
            Assert.That(Convert.ToInt32(rfc822Parts[1]), Is.InRange(1, 31));
            Assert.That(rfc822Parts[2], Is.EqualTo("Nov"));
            Assert.That(rfc822Parts[3], Is.EqualTo("2016"));
            Assert.That(rfc822Parts[4].Length, Is.EqualTo(13));
            Assert.That(rfc822Parts[4].Contains("+") || rfc822Parts[4].Contains("-"), Is.True);
            Assert.That(rfc822Parts[4].Substring(rfc822Parts[4].Length - 4), Is.EqualTo("0000"));
        }

        [Test]
        public void When_ToRfcFormat_is_used_with_unspecified_date_and_hour_offset_Then_resulting_date_should_conform_to_format()
        {
            DateTime validUspecifiedDate = new DateTime(2016, 11, 03, 21, 08, 59, DateTimeKind.Unspecified);
            string rfc822 = validUspecifiedDate.ToRfc822();
            Assert.That(rfc822, Is.Not.Null);
            Assert.That(rfc822, Is.Not.Empty);
            string[] rfc822Parts = rfc822.Split(" ");
            Assert.That(rfc822Parts.Length, Is.EqualTo(5));
            Assert.That(rfc822Parts[0] == "Mon," | rfc822Parts[0] == "Thu," || rfc822Parts[0] == "Wed,", Is.True);
            Assert.That(Convert.ToInt32(rfc822Parts[1]), Is.InRange(1, 31));
            Assert.That(rfc822Parts[2], Is.EqualTo("Nov"));
            Assert.That(rfc822Parts[3], Is.EqualTo("2016"));
            Assert.That(rfc822Parts[4].Length, Is.EqualTo(13));
            Assert.That(rfc822Parts[4].Contains("+") || rfc822Parts[4].Contains("-"), Is.True);
            Assert.That(rfc822Parts[4].Substring(rfc822Parts[4].Length - 4), Is.Not.EqualTo("0000"));
        }

        [Test]
        public void When_ToRfcFormat_is_used_with_local_date_without_hour_offset_Then_resulting_date_should_conform_to_format()
        {
            DateTime validLocalDate = new DateTime(2016, 11, 03, 21, 08, 59, DateTimeKind.Local);
            string rfc822 = validLocalDate.ToRfc822(false);
            Assert.That(rfc822, Is.Not.Null);
            Assert.That(rfc822, Is.Not.Empty);
            string[] rfc822Parts = rfc822.Split(" ");
            Assert.That(rfc822Parts.Length, Is.EqualTo(5));
            Assert.That(rfc822Parts[0] == "Mon," | rfc822Parts[0] == "Thu," || rfc822Parts[0] == "Wed,", Is.True);
            Assert.That(Convert.ToInt32(rfc822Parts[1]), Is.InRange(1, 31));
            Assert.That(rfc822Parts[2], Is.EqualTo("Nov"));
            Assert.That(rfc822Parts[3], Is.EqualTo("2016"));
            Assert.That(rfc822Parts[4].Length, Is.EqualTo(8));
            Assert.That(rfc822Parts[4].Contains("+") || rfc822Parts[4].Contains("-"), Is.False);
            Assert.That(rfc822Parts[4].Substring(rfc822Parts[4].Length - 3), Is.Not.EqualTo("000"));
        }

        [Test]
        public void When_ToRfcFormat_is_used_with_UTC_date_without_hour_offset_Then_resulting_date_should_conform_to_format()
        {
            DateTime validUtcDate = new DateTime(2016, 11, 03, 21, 08, 59, DateTimeKind.Utc);
            string rfc822 = validUtcDate.ToRfc822(false);
            Assert.That(rfc822, Is.Not.Null);
            Assert.That(rfc822, Is.Not.Empty);
            string[] rfc822Parts = rfc822.Split(" ");
            Assert.That(rfc822Parts.Length, Is.EqualTo(5));
            Assert.That(rfc822Parts[0] == "Mon," | rfc822Parts[0] == "Thu," || rfc822Parts[0] == "Wed,", Is.True);
            Assert.That(Convert.ToInt32(rfc822Parts[1]), Is.InRange(1, 31));
            Assert.That(rfc822Parts[2], Is.EqualTo("Nov"));
            Assert.That(rfc822Parts[3], Is.EqualTo("2016"));
            Assert.That(rfc822Parts[4].Length, Is.EqualTo(8));
            Assert.That(rfc822Parts[4].Contains("+") || rfc822Parts[4].Contains("-"), Is.False);
            Assert.That(rfc822Parts[4].Substring(rfc822Parts[4].Length - 3), Is.Not.EqualTo("000"));
        }

        [Test]
        public void When_ToRfcFormat_is_used_with_unspecified_date_without_hour_offset_Then_resulting_date_should_conform_to_format()
        {
            DateTime validUspecifiedDate = new DateTime(2016, 11, 03, 21, 08, 59, DateTimeKind.Unspecified);
            string rfc822 = validUspecifiedDate.ToRfc822(false);
            Assert.That(rfc822, Is.Not.Null);
            Assert.That(rfc822, Is.Not.Empty);
            string[] rfc822Parts = rfc822.Split(" ");
            Assert.That(rfc822Parts.Length, Is.EqualTo(5));
            Assert.That(rfc822Parts[0] == "Mon," | rfc822Parts[0] == "Thu," || rfc822Parts[0] == "Wed,", Is.True);
            Assert.That(Convert.ToInt32(rfc822Parts[1]), Is.InRange(1, 31));
            Assert.That(rfc822Parts[2], Is.EqualTo("Nov"));
            Assert.That(rfc822Parts[3], Is.EqualTo("2016"));
            Assert.That(rfc822Parts[4].Length, Is.EqualTo(8));
            Assert.That(rfc822Parts[4].Contains("+") || rfc822Parts[4].Contains("-"), Is.False);
            Assert.That(rfc822Parts[4].Substring(rfc822Parts[4].Length - 3), Is.Not.EqualTo("000"));
        }

        [Test]
        public void When_ToUnixTime_is_called_on_date_after_UNIX_epoch_start_Then_Unix_time_is_returned()
        {
            Assert.That(new DateTime(2001, 1, 1, 0, 0, 0, DateTimeKind.Utc).ToUnixTime(), Is.EqualTo(978307200));
        }

        [Test]
        public void When_ToUnixTime_is_called_on_date_before_UNIX_epoch_start_Then_Unix_time_is_returned()
        {
            Assert.That(new DateTime(101, 1, 1, 0, 0, 0, DateTimeKind.Utc).ToUnixTime(), Is.EqualTo(-58979923200));
        }

        [Test]
        public void When_ToUnixTime_is_called_on_date_after_UNIX_time_stamp_overflow_Then_Unix_time_is_returned()
        {
            Assert.That(new DateTime(2100, 1, 1, 0, 0, 0, DateTimeKind.Utc).ToUnixTime(), Is.EqualTo(4102444800));
        }
    }
}