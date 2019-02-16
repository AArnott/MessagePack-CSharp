﻿// Copyright (c) Andrew Arnott. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.

using MessagePack.Formatters;
using MessagePack.Internal;
using Microsoft;
using System;
using System.Buffers;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace MessagePack
{
    /// <summary>
    /// A primitive types writer for the MessagePack format.
    /// </summary>
    /// <remarks>
    /// <see href="https://github.com/msgpack/msgpack/blob/master/spec.md">The MessagePack spec.</see>
    /// </remarks>
    public ref struct MessagePackWriter
    {
        /// <summary>
        /// The writer to use.
        /// </summary>
        private BufferWriter writer;

        /// <summary>
        /// Initializes a new instance of the <see cref="MessagePackWriter"/> struct.
        /// </summary>
        /// <param name="writer">The writer to use.</param>
        public MessagePackWriter(IBufferWriter<byte> writer)
        {
            this.writer = new BufferWriter(writer);
            this.OldSpec = false;
            ////this.ForceFullHeaderLength = false;
        }

        /// <summary>
        /// Gets or sets a value indicating whether to write in <see href="https://github.com/msgpack/msgpack/blob/master/spec-old.md">old spec</see> compatibility mode.
        /// </summary>
        public bool OldSpec { get; set; }

        /////// <summary>
        /////// Gets or sets a value indicating whether compressible length headers are written out
        /////// in their longest form instead of their most compact form.
        /////// </summary>
        ////public bool ForceFullHeaderLength { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="MessagePackWriter"/> struct,
        /// with the same settings as this one, but with its own buffer writer.
        /// </summary>
        /// <param name="writer">The writer to use for the new instance.</param>
        /// <returns>The new writer.</returns>
        public MessagePackWriter Clone(IBufferWriter<byte> writer) => new MessagePackWriter(writer)
        {
            OldSpec = this.OldSpec,
            ////ForceFullHeaderLength = this.ForceFullHeaderLength,
        };

        /// <summary>
        /// Ensures everything previously written has been flushed to the underlying <see cref="IBufferWriter{T}"/>.
        /// </summary>
        public void Flush() => this.writer.Commit();

        /// <summary>
        /// Writes a <see cref="MessagePackCode.Nil"/> value.
        /// </summary>
        public void WriteNil()
        {
            var span = writer.GetSpan(1);
            span[0] = MessagePackCode.Nil;
            writer.Advance(1);
        }

        /// <summary>
        /// Copies bytes directly into the message pack writer.
        /// </summary>
        /// <param name="rawMessagePackBlock">The span of bytes to copy from.</param>
        public void WriteRaw(ReadOnlySpan<byte> rawMessagePackBlock) => writer.Write(rawMessagePackBlock);

        /// <summary>
        /// Copies bytes directly into the message pack writer.
        /// </summary>
        /// <param name="rawMessagePackBlock">The span of bytes to copy from.</param>
        public void WriteRaw(ReadOnlySequence<byte> rawMessagePackBlock) => rawMessagePackBlock.CopyTo(ref writer);

        /// <summary>
        /// Write the length of the next array to be written in the most compact form of
        /// <see cref="MessagePackCode.MinFixArray"/>,
        /// <see cref="MessagePackCode.Array16"/>, or
        /// <see cref="MessagePackCode.Array32"/>
        /// </summary>
        /// <param name="count">The number of elements that will be written in the array.</param>
        /// <param name="forceFullHeaderLength"><c>true</c> to force the maximum length header to be written; <c>false</c> to allow the most compact form possible to be used.</param>
        public void WriteArrayHeader(int count, bool forceFullHeaderLength = false) => WriteArrayHeader((uint)count, forceFullHeaderLength);

        /// <summary>
        /// Write the length of the next array to be written in the most compact form of
        /// <see cref="MessagePackCode.MinFixArray"/>,
        /// <see cref="MessagePackCode.Array16"/>, or
        /// <see cref="MessagePackCode.Array32"/>
        /// </summary>
        /// <param name="count">The number of elements that will be written in the array.</param>
        /// <param name="forceFullHeaderLength"><c>true</c> to force the maximum length header to be written; <c>false</c> to allow the most compact form possible to be used.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WriteArrayHeader(uint count, bool forceFullHeaderLength = false)
        {
            if (count <= MessagePackRange.MaxFixArrayCount && !forceFullHeaderLength)
            {
                var span = writer.GetSpan(1);
                span[0] = (byte)(MessagePackCode.MinFixArray | count);
                writer.Advance(1);
            }
            else if (count <= ushort.MaxValue && !forceFullHeaderLength)
            {
                var span = writer.GetSpan(3);
                span[0] = MessagePackCode.Array16;
                WriteBigEndian((ushort)count, span.Slice(1));
                writer.Advance(3);
            }
            else
            {
                var span = writer.GetSpan(5);
                span[0] = MessagePackCode.Array32;
                WriteBigEndian(count, span.Slice(1));
                writer.Advance(5);
            }
        }

        /// <summary>
        /// Write the length of the next map to be written in the most compact form of
        /// <see cref="MessagePackCode.MinFixMap"/>,
        /// <see cref="MessagePackCode.Map16"/>, or
        /// <see cref="MessagePackCode.Map32"/>
        /// </summary>
        /// <param name="count">The number of key=value pairs that will be written in the map.</param>
        /// <param name="forceFullHeaderLength"><c>true</c> to force the maximum length header to be written; <c>false</c> to allow the most compact form possible to be used.</param>
        public void WriteMapHeader(int count, bool forceFullHeaderLength = false) => WriteMapHeader((uint)count, forceFullHeaderLength);

        /// <summary>
        /// Write the length of the next map to be written in the most compact form of
        /// <see cref="MessagePackCode.MinFixMap"/>,
        /// <see cref="MessagePackCode.Map16"/>, or
        /// <see cref="MessagePackCode.Map32"/>
        /// </summary>
        /// <param name="count">The number of key=value pairs that will be written in the map.</param>
        /// <param name="forceFullHeaderLength"><c>true</c> to force the maximum length header to be written; <c>false</c> to allow the most compact form possible to be used.</param>
        public void WriteMapHeader(uint count, bool forceFullHeaderLength = false)
        {
            if (count <= MessagePackRange.MaxFixMapCount && !forceFullHeaderLength)
            {
                var span = writer.GetSpan(1);
                span[0] = (byte)(MessagePackCode.MinFixMap | count);
                writer.Advance(1);
            }
            else if (count <= ushort.MaxValue && !forceFullHeaderLength)
            {
                var span = writer.GetSpan(3);
                span[0] = MessagePackCode.Map16;
                WriteBigEndian((ushort)count, span.Slice(1));
                writer.Advance(3);
            }
            else
            {
                var span = writer.GetSpan(5);
                span[0] = MessagePackCode.Map32;
                WriteBigEndian(count, span.Slice(1));
                writer.Advance(5);
            }
        }

        /// <summary>
        /// Writes a <see cref="byte"/> value using a 1-byte code when possible, otherwise as <see cref="MessagePackCode.UInt8"/>.
        /// </summary>
        /// <param name="value">The value.</param>
        public void Write(byte value)
        {
            if (value <= MessagePackCode.MaxFixInt)
            {
                var span = writer.GetSpan(1);
                span[0] = value;
                writer.Advance(1);
            }
            else
            {
                WriteUInt8(value);
            }
        }

        /// <summary>
        /// Writes a <see cref="byte"/> value using <see cref="MessagePackCode.UInt8"/>.
        /// </summary>
        /// <param name="value">The value.</param>
        public void WriteUInt8(byte value)
        {
            var span = writer.GetSpan(2);
            span[0] = MessagePackCode.UInt8;
            span[1] = value;
            writer.Advance(2);
        }

        /// <summary>
        /// Writes an 8-bit value using a 1-byte code when possible, otherwise as <see cref="MessagePackCode.Int8"/>.
        /// </summary>
        /// <param name="value">The value.</param>
        public void Write(sbyte value)
        {
            if (value < MessagePackRange.MinFixNegativeInt)
            {
                WriteInt8(value);
            }
            else
            {
                var span = writer.GetSpan(1);
                span[0] = unchecked((byte)value);
                writer.Advance(1);
            }
        }

        /// <summary>
        /// Writes an 8-bit value using <see cref="MessagePackCode.Int8"/>.
        /// </summary>
        /// <param name="value">The value.</param>
        public void WriteInt8(sbyte value)
        {
            var span = writer.GetSpan(2);
            span[0] = MessagePackCode.Int8;
            span[1] = unchecked((byte)value);
            writer.Advance(2);
        }

        /// <summary>
        /// Writes a <see cref="ushort"/> value using a 1-byte code when possible, otherwise as <see cref="MessagePackCode.UInt8"/> or <see cref="MessagePackCode.UInt16"/>.
        /// </summary>
        /// <param name="value">The value.</param>
        public void Write(ushort value)
        {
            if (value <= MessagePackRange.MaxFixPositiveInt)
            {
                var span = writer.GetSpan(1);
                span[0] = unchecked((byte)value);
                writer.Advance(1);
            }
            else if (value <= byte.MaxValue)
            {
                var span = writer.GetSpan(2);
                span[0] = MessagePackCode.UInt8;
                span[1] = unchecked((byte)value);
                writer.Advance(2);
            }
            else
            {
                WriteUInt16(value);
            }
        }

        /// <summary>
        /// Writes a <see cref="ushort"/> value using <see cref="MessagePackCode.UInt16"/>.
        /// </summary>
        /// <param name="value">The value.</param>
        public void WriteUInt16(ushort value)
        {
            var span = writer.GetSpan(3);
            span[0] = MessagePackCode.UInt16;
            WriteBigEndian(value, span.Slice(1));
            writer.Advance(3);
        }

        /// <summary>
        /// Writes a <see cref="short"/> using a built-in 1-byte code when within specific MessagePack-supported ranges,
        /// or the most compact of
        /// <see cref="MessagePackCode.UInt8"/>,
        /// <see cref="MessagePackCode.UInt16"/>,
        /// <see cref="MessagePackCode.Int8"/>, or
        /// <see cref="MessagePackCode.Int16"/>
        /// </summary>
        /// <param name="value">The value to write.</param>
        public void Write(short value)
        {
            if (value >= 0)
            {
                Write((ushort)value);
            }
            else
            {
                // negative int(use int)
                if (MessagePackRange.MinFixNegativeInt <= value)
                {
                    var span = writer.GetSpan(1);
                    span[0] = unchecked((byte)value);
                    writer.Advance(1);
                }
                else if (sbyte.MinValue <= value)
                {
                    var span = writer.GetSpan(2);
                    span[0] = MessagePackCode.Int8;
                    span[1] = unchecked((byte)value);
                    writer.Advance(2);
                }
                else
                {
                    WriteInt16(value);
                }
            }
        }

        /// <summary>
        /// Writes a <see cref="short"/> using <see cref="MessagePackCode.Int16"/>.
        /// </summary>
        /// <param name="value">The value to write.</param>
        public void WriteInt16(short value)
        {
            var span = writer.GetSpan(3);
            span[0] = MessagePackCode.Int16;
            WriteBigEndian(value, span.Slice(1));
            writer.Advance(3);
        }

        /// <summary>
        /// Writes an <see cref="uint"/> using a built-in 1-byte code when within specific MessagePack-supported ranges,
        /// or the most compact of
        /// <see cref="MessagePackCode.UInt8"/>,
        /// <see cref="MessagePackCode.UInt16"/>, or
        /// <see cref="MessagePackCode.UInt32"/>
        /// </summary>
        /// <param name="value">The value to write.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Write(uint value)
        {
            if (value <= MessagePackRange.MaxFixPositiveInt)
            {
                var span = writer.GetSpan(1);
                span[0] = unchecked((byte)value);
                writer.Advance(1);
            }
            else if (value <= byte.MaxValue)
            {
                var span = writer.GetSpan(2);
                span[0] = MessagePackCode.UInt8;
                span[1] = unchecked((byte)value);
                writer.Advance(2);
            }
            else if (value <= ushort.MaxValue)
            {
                var span = writer.GetSpan(3);
                span[0] = MessagePackCode.UInt16;
                WriteBigEndian((ushort)value, span.Slice(1));
                writer.Advance(3);
            }
            else
            {
                WriteUInt32(value);
            }
        }

        /// <summary>
        /// Writes an <see cref="uint"/> using <see cref="MessagePackCode.UInt32"/>.
        /// </summary>
        /// <param name="value">The value to write.</param>
        public void WriteUInt32(uint value)
        {
            var span = writer.GetSpan(5);
            span[0] = MessagePackCode.UInt32;
            WriteBigEndian(value, span.Slice(1));
            writer.Advance(5);
        }

        /// <summary>
        /// Writes an <see cref="int"/> using a built-in 1-byte code when within specific MessagePack-supported ranges,
        /// or the most compact of
        /// <see cref="MessagePackCode.UInt8"/>,
        /// <see cref="MessagePackCode.UInt16"/>,
        /// <see cref="MessagePackCode.UInt32"/>,
        /// <see cref="MessagePackCode.Int8"/>,
        /// <see cref="MessagePackCode.Int16"/>,
        /// <see cref="MessagePackCode.Int32"/>
        /// </summary>
        /// <param name="value">The value to write.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Write(int value)
        {
            if (value >= 0)
            {
                Write((uint)value);
            }
            else
            {
                // negative int(use int)
                if (MessagePackRange.MinFixNegativeInt <= value)
                {
                    var span = writer.GetSpan(1);
                    span[0] = unchecked((byte)value);
                    writer.Advance(1);
                }
                else if (sbyte.MinValue <= value)
                {
                    var span = writer.GetSpan(2);
                    span[0] = MessagePackCode.Int8;
                    span[1] = unchecked((byte)value);
                    writer.Advance(2);
                }
                else if (short.MinValue <= value)
                {
                    var span = writer.GetSpan(3);
                    span[0] = MessagePackCode.Int16;
                    WriteBigEndian((short)value, span.Slice(1));
                    writer.Advance(3);
                }
                else
                {
                    WriteInt32(value);
                }
            }
        }

        /// <summary>
        /// Writes an <see cref="int"/> using <see cref="MessagePackCode.Int32"/>.
        /// </summary>
        /// <param name="value">The value to write.</param>
        public void WriteInt32(int value)
        {
            var span = writer.GetSpan(5);
            span[0] = MessagePackCode.Int32;
            WriteBigEndian(value, span.Slice(1));
            writer.Advance(5);
        }

        /// <summary>
        /// Writes an <see cref="ulong"/> using a built-in 1-byte code when within specific MessagePack-supported ranges,
        /// or the most compact of
        /// <see cref="MessagePackCode.UInt8"/>,
        /// <see cref="MessagePackCode.UInt16"/>,
        /// <see cref="MessagePackCode.UInt32"/>,
        /// <see cref="MessagePackCode.Int8"/>,
        /// <see cref="MessagePackCode.Int16"/>,
        /// <see cref="MessagePackCode.Int32"/>
        /// </summary>
        /// <param name="value">The value to write.</param>
        public void Write(ulong value)
        {
            if (value <= MessagePackRange.MaxFixPositiveInt)
            {
                var span = writer.GetSpan(1);
                span[0] = unchecked((byte)value);
                writer.Advance(1);
            }
            else if (value <= byte.MaxValue)
            {
                var span = writer.GetSpan(2);
                span[0] = MessagePackCode.UInt8;
                span[1] = unchecked((byte)value);
                writer.Advance(2);
            }
            else if (value <= ushort.MaxValue)
            {
                var span = writer.GetSpan(3);
                span[0] = MessagePackCode.UInt16;
                WriteBigEndian((ushort)value, span.Slice(1));
                writer.Advance(3);
            }
            else if (value <= uint.MaxValue)
            {
                var span = writer.GetSpan(5);
                span[0] = MessagePackCode.UInt32;
                WriteBigEndian((uint)value, span.Slice(1));
                writer.Advance(5);
            }
            else
            {
                WriteUInt64(value);
            }
        }

        /// <summary>
        /// Writes an <see cref="ulong"/> using <see cref="MessagePackCode.Int32"/>.
        /// </summary>
        /// <param name="value">The value to write.</param>
        public void WriteUInt64(ulong value)
        {
            var span = writer.GetSpan(9);
            span[0] = MessagePackCode.UInt64;
            WriteBigEndian(value, span.Slice(1));
            writer.Advance(9);
        }

        /// <summary>
        /// Writes an <see cref="long"/> using a built-in 1-byte code when within specific MessagePack-supported ranges,
        /// or the most compact of
        /// <see cref="MessagePackCode.UInt8"/>,
        /// <see cref="MessagePackCode.UInt16"/>,
        /// <see cref="MessagePackCode.UInt32"/>,
        /// <see cref="MessagePackCode.UInt64"/>,
        /// <see cref="MessagePackCode.Int8"/>,
        /// <see cref="MessagePackCode.Int16"/>,
        /// <see cref="MessagePackCode.Int32"/>,
        /// <see cref="MessagePackCode.Int64"/>
        /// </summary>
        /// <param name="value">The value to write.</param>
        public void Write(long value)
        {
            if (value >= 0)
            {
                Write((ulong)value);
            }
            else
            {
                // negative int(use int)
                if (MessagePackRange.MinFixNegativeInt <= value)
                {
                    var span = writer.GetSpan(1);
                    span[0] = unchecked((byte)value);
                    writer.Advance(1);
                }
                else if (sbyte.MinValue <= value)
                {
                    var span = writer.GetSpan(2);
                    span[0] = MessagePackCode.Int8;
                    span[1] = unchecked((byte)value);
                    writer.Advance(2);
                }
                else if (short.MinValue <= value)
                {
                    var span = writer.GetSpan(3);
                    span[0] = MessagePackCode.Int16;
                    WriteBigEndian((short)value, span.Slice(1));
                    writer.Advance(3);
                }
                else if (int.MinValue <= value)
                {
                    var span = writer.GetSpan(5);
                    span[0] = MessagePackCode.Int32;
                    WriteBigEndian((int)value, span.Slice(1));
                    writer.Advance(5);
                }
                else
                {
                    WriteInt64(value);
                }
            }
        }

        /// <summary>
        /// Writes an <see cref="long"/> using a built-in 1-byte code when within specific MessagePack-supported ranges,
        /// or the most compact of
        /// <see cref="MessagePackCode.UInt8"/>,
        /// <see cref="MessagePackCode.UInt16"/>,
        /// <see cref="MessagePackCode.UInt32"/>,
        /// <see cref="MessagePackCode.UInt64"/>,
        /// <see cref="MessagePackCode.Int8"/>,
        /// <see cref="MessagePackCode.Int16"/>,
        /// <see cref="MessagePackCode.Int32"/>,
        /// <see cref="MessagePackCode.Int64"/>
        /// </summary>
        /// <param name="value">The value to write.</param>
        public void WriteInt64(long value)
        {
            var span = writer.GetSpan(9);
            span[0] = MessagePackCode.Int64;
            WriteBigEndian(value, span.Slice(1));
            writer.Advance(9);
        }

        /// <summary>
        /// Writes a <see cref="bool"/> value using either <see cref="MessagePackCode.True"/> or <see cref="MessagePackCode.False"/>.
        /// </summary>
        /// <param name="value">The value.</param>
        public void Write(bool value)
        {
            var span = writer.GetSpan(1);
            span[0] = value ? MessagePackCode.True : MessagePackCode.False;
            writer.Advance(1);
        }

        /// <summary>
        /// Writes a <see cref="char"/> value using a 1-byte code when possible, otherwise as <see cref="MessagePackCode.UInt8"/> or <see cref="MessagePackCode.UInt16"/>.
        /// </summary>
        /// <param name="value">The value.</param>
        public void Write(char value) => this.Write((ushort)value);

        /// <summary>
        /// Writes a <see cref="MessagePackCode.Float32"/> value.
        /// </summary>
        /// <param name="value">The value.</param>
        public void Write(float value)
        {
            var span = writer.GetSpan(5);

            span[0] = MessagePackCode.Float32;

            var num = new Float32Bits(value);
            if (BitConverter.IsLittleEndian)
            {
                span[1] = num.Byte3;
                span[2] = num.Byte2;
                span[3] = num.Byte1;
                span[4] = num.Byte0;
            }
            else
            {
                span[1] = num.Byte0;
                span[2] = num.Byte1;
                span[3] = num.Byte2;
                span[4] = num.Byte3;
            }

            writer.Advance(5);
        }

        /// <summary>
        /// Writes a <see cref="MessagePackCode.Float64"/> value.
        /// </summary>
        /// <param name="value">The value.</param>
        public void Write(double value)
        {
            var span = writer.GetSpan(9);

            span[0] = MessagePackCode.Float64;

            var num = new Float64Bits(value);
            if (BitConverter.IsLittleEndian)
            {
                span[1] = num.Byte7;
                span[2] = num.Byte6;
                span[3] = num.Byte5;
                span[4] = num.Byte4;
                span[5] = num.Byte3;
                span[6] = num.Byte2;
                span[7] = num.Byte1;
                span[8] = num.Byte0;
            }
            else
            {
                span[1] = num.Byte0;
                span[2] = num.Byte1;
                span[3] = num.Byte2;
                span[4] = num.Byte3;
                span[5] = num.Byte4;
                span[6] = num.Byte5;
                span[7] = num.Byte6;
                span[8] = num.Byte7;
            }

            writer.Advance(9);
        }

        /// <summary>
        /// Writes a <see cref="DateTime"/> using the message code <see cref="ReservedMessagePackExtensionTypeCode.DateTime"/>.
        /// </summary>
        /// <param name="dateTime">The value to write.</param>
        public void Write(DateTime dateTime)
        {
            // Timestamp spec
            // https://github.com/msgpack/msgpack/pull/209
            // FixExt4(-1) => seconds |  [1970-01-01 00:00:00 UTC, 2106-02-07 06:28:16 UTC) range
            // FixExt8(-1) => nanoseconds + seconds | [1970-01-01 00:00:00.000000000 UTC, 2514-05-30 01:53:04.000000000 UTC) range
            // Ext8(12,-1) => nanoseconds + seconds | [-584554047284-02-23 16:59:44 UTC, 584554051223-11-09 07:00:16.000000000 UTC) range
            dateTime = dateTime.ToUniversalTime();

            var secondsSinceBclEpoch = dateTime.Ticks / TimeSpan.TicksPerSecond;
            var seconds = secondsSinceBclEpoch - DateTimeConstants.BclSecondsAtUnixEpoch;
            var nanoseconds = (dateTime.Ticks % TimeSpan.TicksPerSecond) * DateTimeConstants.NanosecondsPerTick;

            // reference pseudo code.
            /*
            struct timespec {
                long tv_sec;  // seconds
                long tv_nsec; // nanoseconds
            } time;
            if ((time.tv_sec >> 34) == 0)
            {
                uint64_t data64 = (time.tv_nsec << 34) | time.tv_sec;
                if (data & 0xffffffff00000000L == 0)
                {
                    // timestamp 32
                    uint32_t data32 = data64;
                    serialize(0xd6, -1, data32)
                }
                else
                {
                    // timestamp 64
                    serialize(0xd7, -1, data64)
                }
            }
            else
            {
                // timestamp 96
                serialize(0xc7, 12, -1, time.tv_nsec, time.tv_sec)
            }
            */

            if ((seconds >> 34) == 0)
            {
                var data64 = unchecked((ulong)((nanoseconds << 34) | seconds));
                if ((data64 & 0xffffffff00000000L) == 0)
                {
                    // timestamp 32(seconds in 32-bit unsigned int)
                    var data32 = (UInt32)data64;
                    var span = writer.GetSpan(6);
                    span[0] = MessagePackCode.FixExt4;
                    span[1] = unchecked((byte)ReservedMessagePackExtensionTypeCode.DateTime);
                    WriteBigEndian(data32, span.Slice(2));
                    writer.Advance(6);
                }
                else
                {
                    // timestamp 64(nanoseconds in 30-bit unsigned int | seconds in 34-bit unsigned int)
                    var span = writer.GetSpan(10);
                    span[0] = MessagePackCode.FixExt8;
                    span[1] = unchecked((byte)ReservedMessagePackExtensionTypeCode.DateTime);
                    WriteBigEndian(data64, span.Slice(2));
                    writer.Advance(10);
                }
            }
            else
            {
                // timestamp 96( nanoseconds in 32-bit unsigned int | seconds in 64-bit signed int )
                var span = writer.GetSpan(15);
                span[0] = MessagePackCode.Ext8;
                span[1] = 12;
                span[2] = unchecked((byte)ReservedMessagePackExtensionTypeCode.DateTime);
                WriteBigEndian((uint)nanoseconds, span.Slice(3));
                WriteBigEndian(seconds, span.Slice(7));
                writer.Advance(15);
            }
        }

        /// <summary>
        /// Writes a span of bytes, prefixed with a length encoded as the smallest fitting from:
        /// <see cref="MessagePackCode.Bin8"/>,
        /// <see cref="MessagePackCode.Bin16"/>, or
        /// <see cref="MessagePackCode.Bin32"/>,
        /// </summary>
        /// <param name="src">The span of bytes to write.</param>
        /// <param name="forceFullHeaderLength"><c>true</c> to force the maximum length header to be written; <c>false</c> to allow the most compact form possible to be used.</param>
        public void Write(ReadOnlySpan<byte> src, bool forceFullHeaderLength = false)
        {
            if (this.OldSpec)
            {
                WriteString(src, forceFullHeaderLength);
                return;
            }

            if (src.Length <= byte.MaxValue && !forceFullHeaderLength)
            {
                var size = src.Length + 2;
                var span = writer.GetSpan(size);

                span[0] = MessagePackCode.Bin8;
                span[1] = (byte)src.Length;

                src.CopyTo(span.Slice(2));
                writer.Advance(size);
            }
            else if (src.Length <= UInt16.MaxValue && !forceFullHeaderLength)
            {
                var size = src.Length + 3;
                var span = writer.GetSpan(size);

                span[0] = MessagePackCode.Bin16;
                WriteBigEndian((ushort)src.Length, span.Slice(1));

                src.CopyTo(span.Slice(3));
                writer.Advance(size);
            }
            else
            {
                var size = src.Length + 5;
                var span = writer.GetSpan(size);

                span[0] = MessagePackCode.Bin32;
                WriteBigEndian(src.Length, span.Slice(1));

                src.CopyTo(span.Slice(5));
                writer.Advance(size);
            }
        }

        /// <summary>
        /// Writes a sequence of bytes, prefixed with a length encoded as the smallest fitting from:
        /// <see cref="MessagePackCode.Bin8"/>,
        /// <see cref="MessagePackCode.Bin16"/>, or
        /// <see cref="MessagePackCode.Bin32"/>,
        /// </summary>
        /// <param name="src">The span of bytes to write.</param>
        /// <param name="forceFullHeaderLength"><c>true</c> to force the maximum length header to be written; <c>false</c> to allow the most compact form possible to be used.</param>
        public void Write(ReadOnlySequence<byte> src, bool forceFullHeaderLength = false)
        {
            if (this.OldSpec)
            {
                WriteString(src, forceFullHeaderLength);
                return;
            }

            if (src.Length <= byte.MaxValue && !forceFullHeaderLength)
            {
                var size = (int)src.Length + 2;
                var span = writer.GetSpan(size);

                span[0] = MessagePackCode.Bin8;
                span[1] = (byte)src.Length;

                src.CopyTo(span.Slice(2));
                writer.Advance(size);
            }
            else if (src.Length <= UInt16.MaxValue && !forceFullHeaderLength)
            {
                var size = (int)src.Length + 3;
                var span = writer.GetSpan(size);

                span[0] = MessagePackCode.Bin16;
                WriteBigEndian((ushort)src.Length, span.Slice(1));

                src.CopyTo(span.Slice(3));
                writer.Advance(size);
            }
            else
            {
                var size = (int)src.Length + 5;
                var span = writer.GetSpan(size);

                span[0] = MessagePackCode.Bin32;
                WriteBigEndian(src.Length, span.Slice(1));

                src.CopyTo(span.Slice(5));
                writer.Advance(size);
            }
        }

        /// <summary>
        /// Writes out an array of bytes that (may) represent a UTF-8 encoded string, prefixed with the length using one of these message codes:
        /// <see cref="MessagePackCode.MinFixStr"/>,
        /// <see cref="MessagePackCode.Str8"/>,
        /// <see cref="MessagePackCode.Str16"/>,
        /// <see cref="MessagePackCode.Str32"/>,
        /// </summary>
        /// <param name="utf8stringBytes">The bytes to write.</param>
        /// <param name="forceFullHeaderLength"><c>true</c> to force the maximum length header to be written; <c>false</c> to allow the most compact form possible to be used.</param>
        public void WriteString(ReadOnlySequence<byte> utf8stringBytes, bool forceFullHeaderLength = false)
        {
            var byteCount = (int)utf8stringBytes.Length;
            if (byteCount <= MessagePackRange.MaxFixStringLength && !forceFullHeaderLength)
            {
                var span = writer.GetSpan(byteCount + 1);
                span[0] = (byte)(MessagePackCode.MinFixStr | byteCount);
                utf8stringBytes.CopyTo(span.Slice(1));
                writer.Advance(byteCount + 1);
            }
            else if (byteCount <= byte.MaxValue && !this.OldSpec && !forceFullHeaderLength)
            {
                var span = writer.GetSpan(byteCount + 2);
                span[0] = MessagePackCode.Str8;
                span[1] = unchecked((byte)byteCount);
                utf8stringBytes.CopyTo(span.Slice(2));
                writer.Advance(byteCount + 2);
            }
            else if (byteCount <= ushort.MaxValue && !forceFullHeaderLength)
            {
                var span = writer.GetSpan(byteCount + 3);
                span[0] = MessagePackCode.Str16;
                WriteBigEndian((ushort)byteCount, span.Slice(1));
                utf8stringBytes.CopyTo(span.Slice(3));
                writer.Advance(byteCount + 3);
            }
            else
            {
                var span = writer.GetSpan(byteCount + 5);
                span[0] = MessagePackCode.Str32;
                WriteBigEndian(byteCount, span.Slice(1));
                utf8stringBytes.CopyTo(span.Slice(5));
                writer.Advance(byteCount + 5);
            }
        }

        /// <summary>
        /// Writes out an array of bytes that (may) represent a UTF-8 encoded string, prefixed with the length using one of these message codes:
        /// <see cref="MessagePackCode.MinFixStr"/>,
        /// <see cref="MessagePackCode.Str8"/>,
        /// <see cref="MessagePackCode.Str16"/>,
        /// <see cref="MessagePackCode.Str32"/>,
        /// </summary>
        /// <param name="utf8stringBytes">The bytes to write.</param>
        /// <param name="forceFullHeaderLength"><c>true</c> to force the maximum length header to be written; <c>false</c> to allow the most compact form possible to be used.</param>
        public void WriteString(ReadOnlySpan<byte> utf8stringBytes, bool forceFullHeaderLength = false)
        {
            var byteCount = utf8stringBytes.Length;
            if (byteCount <= MessagePackRange.MaxFixStringLength && !forceFullHeaderLength)
            {
                var span = writer.GetSpan(byteCount + 1);
                span[0] = (byte)(MessagePackCode.MinFixStr | byteCount);
                utf8stringBytes.CopyTo(span.Slice(1));
                writer.Advance(byteCount + 1);
            }
            else if (byteCount <= byte.MaxValue && !this.OldSpec && !forceFullHeaderLength)
            {
                var span = writer.GetSpan(byteCount + 2);
                span[0] = MessagePackCode.Str8;
                span[1] = unchecked((byte)byteCount);
                utf8stringBytes.CopyTo(span.Slice(2));
                writer.Advance(byteCount + 2);
            }
            else if (byteCount <= ushort.MaxValue && !forceFullHeaderLength)
            {
                var span = writer.GetSpan(byteCount + 3);
                span[0] = MessagePackCode.Str16;
                WriteBigEndian((ushort)byteCount, span.Slice(1));
                utf8stringBytes.CopyTo(span.Slice(3));
                writer.Advance(byteCount + 3);
            }
            else
            {
                var span = writer.GetSpan(byteCount + 5);
                span[0] = MessagePackCode.Str32;
                WriteBigEndian(byteCount, span.Slice(1));
                utf8stringBytes.CopyTo(span.Slice(5));
                writer.Advance(byteCount + 5);
            }
        }

        /// <summary>
        /// Writes out a <see cref="string"/>, prefixed with the length using one of these message codes:
        /// <see cref="MessagePackCode.MinFixStr"/>,
        /// <see cref="MessagePackCode.Str8"/>,
        /// <see cref="MessagePackCode.Str16"/>,
        /// <see cref="MessagePackCode.Str32"/>,
        /// </summary>
        /// <param name="value">The value to write. Must not be null.</param>
        /// <param name="forceFullHeaderLength"><c>true</c> to force the maximum length header to be written; <c>false</c> to allow the most compact form possible to be used.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Write(string value, bool forceFullHeaderLength = false)
        {
            if (value == null)
            {
                ThrowArgumentNullException(nameof(value));
            }

            Write(value.AsSpan(), forceFullHeaderLength);
        }

        /// <summary>
        /// Writes out a <see cref="string"/>, prefixed with the length using one of these message codes:
        /// <see cref="MessagePackCode.MinFixStr"/>,
        /// <see cref="MessagePackCode.Str8"/>,
        /// <see cref="MessagePackCode.Str16"/>,
        /// <see cref="MessagePackCode.Str32"/>,
        /// </summary>
        /// <param name="value">The value to write.</param>
        /// <param name="forceFullHeaderLength"><c>true</c> to force the maximum length header to be written; <c>false</c> to allow the most compact form possible to be used.</param>
        public void Write(ReadOnlySpan<char> value, bool forceFullHeaderLength = false)
        {
            // MaxByteCount -> WritePrefix -> GetBytes has some overheads of `MaxByteCount`
            // solves heuristic length check

            // ensure buffer by MaxByteCount(faster than GetByteCount)
            var span = writer.GetSpan(StringEncoding.UTF8.GetMaxByteCount(value.Length) + 5);

            int useOffset;
            if (value.Length <= MessagePackRange.MaxFixStringLength && !forceFullHeaderLength)
            {
                useOffset = 1;
            }
            else if (value.Length <= byte.MaxValue && !this.OldSpec && !forceFullHeaderLength)
            {
                useOffset = 2;
            }
            else if (value.Length <= ushort.MaxValue && !forceFullHeaderLength)
            {
                useOffset = 3;
            }
            else
            {
                useOffset = 5;
            }

            // skip length area
            var byteCount = StringEncoding.UTF8.GetBytes(value, span.Slice(useOffset));

            // move body and write prefix
            if (byteCount <= MessagePackRange.MaxFixStringLength && !forceFullHeaderLength)
            {
                if (useOffset != 1)
                {
                    span.Slice(useOffset, byteCount).CopyTo(span.Slice(1));
                }
                span[0] = (byte)(MessagePackCode.MinFixStr | byteCount);
                writer.Advance(byteCount + 1);
            }
            else if (byteCount <= byte.MaxValue && !this.OldSpec && !forceFullHeaderLength)
            {
                if (useOffset != 2)
                {
                    span.Slice(useOffset, byteCount).CopyTo(span.Slice(2));
                }

                span[0] = MessagePackCode.Str8;
                span[1] = unchecked((byte)byteCount);
                writer.Advance(byteCount + 2);
            }
            else if (byteCount <= ushort.MaxValue && !forceFullHeaderLength)
            {
                if (useOffset != 3)
                {
                    span.Slice(useOffset, byteCount).CopyTo(span.Slice(3));
                }

                span[0] = MessagePackCode.Str16;
                WriteBigEndian((ushort)byteCount, span.Slice(1));
                writer.Advance(byteCount + 3);
            }
            else
            {
                if (useOffset != 5)
                {
                    span.Slice(useOffset, byteCount).CopyTo(span.Slice(5));
                }

                span[0] = MessagePackCode.Str32;
                WriteBigEndian(byteCount, span.Slice(1));
                writer.Advance(byteCount + 5);
            }
        }

        /// <summary>
        /// Writes the extension format header, using the smallest one of these codes:
        /// <see cref="MessagePackCode.FixExt1"/>,
        /// <see cref="MessagePackCode.FixExt2"/>,
        /// <see cref="MessagePackCode.FixExt4"/>,
        /// <see cref="MessagePackCode.FixExt8"/>,
        /// <see cref="MessagePackCode.FixExt16"/>,
        /// <see cref="MessagePackCode.Ext8"/>,
        /// <see cref="MessagePackCode.Ext16"/>, or
        /// <see cref="MessagePackCode.Ext32"/>.
        /// </summary>
        /// <param name="extensionHeader">The extension header.</param>
        /// <param name="forceFullHeaderLength"><c>true</c> to force the maximum length header to be written; <c>false</c> to allow the most compact form possible to be used.</param>
        public void WriteExtensionFormatHeader(ExtensionHeader extensionHeader, bool forceFullHeaderLength = false)
        {
            int dataLength = (int)extensionHeader.Length;
            byte typeCode = (byte)extensionHeader.TypeCode;
            switch (dataLength)
            {
                case 1 when !forceFullHeaderLength:
                    var span = writer.GetSpan(2);
                    span[0] = MessagePackCode.FixExt1;
                    span[1] = unchecked(typeCode);
                    writer.Advance(2);
                    return;
                case 2 when !forceFullHeaderLength:
                    span = writer.GetSpan(2);
                    span[0] = MessagePackCode.FixExt2;
                    span[1] = unchecked(typeCode);
                    writer.Advance(2);
                    return;
                case 4 when !forceFullHeaderLength:
                    span = writer.GetSpan(2);
                    span[0] = MessagePackCode.FixExt4;
                    span[1] = unchecked(typeCode);
                    writer.Advance(2);
                    return;
                case 8 when !forceFullHeaderLength:
                    span = writer.GetSpan(2);
                    span[0] = MessagePackCode.FixExt8;
                    span[1] = unchecked(typeCode);
                    writer.Advance(2);
                    return;
                case 16 when !forceFullHeaderLength:
                    span = writer.GetSpan(2);
                    span[0] = MessagePackCode.FixExt16;
                    span[1] = unchecked(typeCode);
                    writer.Advance(2);
                    return;
                default:
                    unchecked
                    {
                        if (dataLength <= byte.MaxValue && !forceFullHeaderLength)
                        {
                            span = writer.GetSpan(dataLength + 3);
                            span[0] = MessagePackCode.Ext8;
                            span[1] = unchecked((byte)dataLength);
                            span[2] = unchecked(typeCode);
                            writer.Advance(3);
                        }
                        else if (dataLength <= UInt16.MaxValue && !forceFullHeaderLength)
                        {
                            span = writer.GetSpan(dataLength + 4);
                            span[0] = MessagePackCode.Ext16;
                            WriteBigEndian((ushort)dataLength, span.Slice(1));
                            span[3] = unchecked(typeCode);
                            writer.Advance(4);
                        }
                        else
                        {
                            span = writer.GetSpan(dataLength + 6);
                            span[0] = MessagePackCode.Ext32;
                            WriteBigEndian(dataLength, span.Slice(1));
                            span[5] = unchecked(typeCode);
                            writer.Advance(6);
                        }
                    }

                    break;
            }
        }

        /// <summary>
        /// Writes an extension format, using the smallest one of these codes:
        /// <see cref="MessagePackCode.FixExt1"/>,
        /// <see cref="MessagePackCode.FixExt2"/>,
        /// <see cref="MessagePackCode.FixExt4"/>,
        /// <see cref="MessagePackCode.FixExt8"/>,
        /// <see cref="MessagePackCode.FixExt16"/>,
        /// <see cref="MessagePackCode.Ext8"/>,
        /// <see cref="MessagePackCode.Ext16"/>, or
        /// <see cref="MessagePackCode.Ext32"/>.
        /// </summary>
        /// <param name="extensionData">The extension data.</param>
        /// <param name="forceFullHeaderLength"><c>true</c> to force the maximum length header to be written; <c>false</c> to allow the most compact form possible to be used.</param>
        public void WriteExtensionFormat(ExtensionResult extensionData, bool forceFullHeaderLength = false)
        {
            WriteExtensionFormatHeader(extensionData.Header, forceFullHeaderLength);
            WriteRaw(extensionData.Data);
        }

        /// <summary>
        /// Writes a 16-bit integer in big endian format.
        /// </summary>
        /// <param name="value">The integer.</param>
        internal void WriteBigEndian(ushort value)
        {
            var span = writer.GetSpan(2);
            WriteBigEndian(value, span);
            writer.Advance(2);
        }

        /// <summary>
        /// Writes a 32-bit integer in big endian format.
        /// </summary>
        /// <param name="value">The integer.</param>
        internal void WriteBigEndian(uint value)
        {
            var span = writer.GetSpan(4);
            WriteBigEndian(value, span);
            writer.Advance(4);
        }

        /// <summary>
        /// Writes a 64-bit integer in big endian format.
        /// </summary>
        /// <param name="value">The integer.</param>
        internal void WriteBigEndian(ulong value)
        {
            var span = writer.GetSpan(8);
            WriteBigEndian(value, span);
            writer.Advance(8);
        }

        internal Span<byte> GetSpan(int length) => writer.GetSpan(length);

        internal void Advance(int length) => writer.Advance(length);

        private static void WriteBigEndian(short value, Span<byte> span) => WriteBigEndian(unchecked((ushort)value), span);

        private static void WriteBigEndian(int value, Span<byte> span) => WriteBigEndian(unchecked((uint)value), span);

        private static void WriteBigEndian(long value, Span<byte> span) => WriteBigEndian(unchecked((ulong)value), span);

        private static void WriteBigEndian(ushort value, Span<byte> span)
        {
            unchecked
            {
                // Write to highest index first so the JIT skips bounds checks on subsequent writes.
                span[1] = (byte)value;
                span[0] = (byte)(value >> 8);
            }
        }

        private static void WriteBigEndian(uint value, Span<byte> span)
        {
            unchecked
            {
                // Write to highest index first so the JIT skips bounds checks on subsequent writes.
                span[3] = (byte)value;
                span[2] = (byte)(value >> 8);
                span[1] = (byte)(value >> 16);
                span[0] = (byte)(value >> 24);
            }
        }

        private static void WriteBigEndian(ulong value, Span<byte> span)
        {
            unchecked
            {
                // Write to highest index first so the JIT skips bounds checks on subsequent writes.
                span[7] = (byte)value;
                span[6] = (byte)(value >> 8);
                span[5] = (byte)(value >> 16);
                span[4] = (byte)(value >> 24);
                span[3] = (byte)(value >> 32);
                span[2] = (byte)(value >> 40);
                span[1] = (byte)(value >> 48);
                span[0] = (byte)(value >> 56);
            }
        }

        private static void ThrowArgumentNullException(string parameterName) => throw new ArgumentNullException(parameterName);
    }
}
