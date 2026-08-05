using System;
using System.IO;

namespace BlubLib.IO
{
    // From http://referencesource.microsoft.com/#PresentationCore/Shared/MS/Internal/Ink/BitStream.cs,6a7d84e516b71ab0
    public class BitStreamReader
    {
        private readonly byte[] _byteArray;
        private uint _bufferLengthInBits;
        private int _byteArrayIndex;
        private byte _partialByte;
        private int _cbitsInPartialByte;

        public bool EndOfStream => 0 == _bufferLengthInBits;
        public int CurrentIndex => _byteArrayIndex - 1;

        public BitStreamReader(byte[] buffer)
        {
            _byteArray = buffer;
            _bufferLengthInBits = (uint)buffer.Length * 8;
        }

        public BitStreamReader(byte[] buffer, int startIndex)
        {
            if (startIndex < 0 || startIndex >= buffer.Length)
                throw new ArgumentOutOfRangeException(nameof(startIndex));

            _byteArray = buffer;
            _byteArrayIndex = startIndex;
            _bufferLengthInBits = (uint)(buffer.Length - startIndex) * 8;
        }

        public BitStreamReader(byte[] buffer, uint bufferLengthInBits)
            : this(buffer)
        {
            if (bufferLengthInBits > (buffer.Length * 8))
                throw new ArgumentOutOfRangeException(nameof(bufferLengthInBits));

            _bufferLengthInBits = bufferLengthInBits;
        }

        public long ReadUInt64(int countOfBits)
        {
            // we only support 1-64 bits currently, not multiple bytes, and not 0 bits
            if (countOfBits > 64 || countOfBits <= 0)
                throw new ArgumentOutOfRangeException(nameof(countOfBits));

            long retVal = 0;
            while (countOfBits > 0)
            {
                var countToRead = 8;
                if (countOfBits < 8)
                {
                    countToRead = countOfBits;
                }
                //make room 
                retVal <<= countToRead;
                var b = ReadByte(countToRead);
                retVal |= b;
                countOfBits -= countToRead;
            }
            return retVal;
        }

        public ushort ReadUInt16(int countOfBits)
        {
            // we only support 1-16 bits currently, not multiple bytes, and not 0 bits
            if (countOfBits > 16 || countOfBits <= 0)
                throw new ArgumentOutOfRangeException(nameof(countOfBits));

            ushort retVal = 0;
            while (countOfBits > 0)
            {
                var countToRead = 8;
                if (countOfBits < 8)
                {
                    countToRead = countOfBits;
                }
                //make room
                retVal <<= countToRead;
                var b = ReadByte(countToRead);
                retVal |= b;
                countOfBits -= countToRead;
            }
            return retVal;
        }
        
        public uint ReadUInt16Reverse(int countOfBits)
        {
            // we only support 1-8 bits currently, not multiple bytes, and not 0 bits 
            if (countOfBits > 16 || countOfBits <= 0)
                throw new ArgumentOutOfRangeException(nameof(countOfBits));

            ushort retVal = 0;
            var fullBytesRead = 0;
            while (countOfBits > 0)
            {
                var countToRead = 8;
                if (countOfBits < 8)
                {
                    countToRead = countOfBits;
                }
                //make room
                ushort b = ReadByte(countToRead);
                b <<= (fullBytesRead * 8);
                retVal |= b;
                fullBytesRead++;
                countOfBits -= countToRead;
            }
            return retVal;
        }

        public uint ReadUInt32(int countOfBits)
        {
            // we only support 1-8 bits currently, not multiple bytes, and not 0 bits 
            if (countOfBits > 32 || countOfBits <= 0)
                throw new ArgumentOutOfRangeException(nameof(countOfBits));

            uint retVal = 0;
            while (countOfBits > 0)
            {
                var countToRead = 8;
                if (countOfBits < 8)
                {
                    countToRead = countOfBits;
                }
                //make room
                retVal <<= countToRead;
                var b = ReadByte(countToRead);
                retVal |= b;
                countOfBits -= countToRead;
            }
            return retVal;
        }

        public uint ReadUInt32Reverse(int countOfBits)
        {
            // we only support 1-8 bits currently, not multiple bytes, and not 0 bits
            if (countOfBits > 32 || countOfBits <= 0)
                throw new ArgumentOutOfRangeException(nameof(countOfBits));

            uint retVal = 0;
            var fullBytesRead = 0;
            while (countOfBits > 0)
            {
                var countToRead = 8;
                if (countOfBits < 8)
                {
                    countToRead = countOfBits;
                }
                //make room 
                uint b = ReadByte(countToRead);
                b <<= (fullBytesRead * 8);
                retVal |= b;
                fullBytesRead++;
                countOfBits -= countToRead;
            }
            return retVal;
        }

        public bool ReadBit()
        {
            var b = ReadByte(1);
            return ((b & 1) == 1);
        }

        public byte ReadByte(int countOfBits)
        {
            // if the end of the stream has been reached, then throw an exception
            if (EndOfStream)
                throw new EndOfStreamException();

            // we only support 1-8 bits currently, not multiple bytes, and not 0 bits 
            if (countOfBits > 8 || countOfBits <= 0)
                throw new ArgumentOutOfRangeException(nameof(countOfBits));

            if (countOfBits > _bufferLengthInBits)
                throw new ArgumentOutOfRangeException(nameof(countOfBits));

            _bufferLengthInBits -= (uint)countOfBits;

            // initialize return byte to 0 before reading from the cache
            byte returnByte;

            // if the partial bit cache contains more bits than requested, then read the
            //      cache only 
            if (_cbitsInPartialByte >= countOfBits)
            {
                // retrieve the requested count of most significant bits from the cache 
                //      and store them in the least significant positions in the return byte
                var rightShiftPartialByteBy = 8 - countOfBits;
                returnByte = (byte)(_partialByte >> rightShiftPartialByteBy);

                // reposition any unused portion of the cache in the most significant part of the bit cache
                unchecked // disable overflow checking since we are intentionally throwing away 
                          //  the significant bits 
                {
                    _partialByte <<= countOfBits;
                }
                // update the bit count in the cache
                _cbitsInPartialByte -= countOfBits;
            }
            // otherwise, we need to retrieve more full bytes from the stream
            else
            {
                // retrieve the next full byte from the stream
                var nextByte = _byteArray[_byteArrayIndex];
                _byteArrayIndex++;

                //right shift partial byte to get it ready to or with the partial next byte
                var rightShiftPartialByteBy = 8 - countOfBits;
                returnByte = (byte)(_partialByte >> rightShiftPartialByteBy);

                // now copy the remaining chunk of the newly retrieved full byte 
                var rightShiftNextByteBy = Math.Abs((countOfBits - _cbitsInPartialByte) - 8);
                returnByte |= (byte)(nextByte >> rightShiftNextByteBy);

                // update the partial bit cache with the remainder of the newly retrieved full byte
                unchecked // disable overflow checking since we are intentionally throwing away
                          //  the significant bits 
                {
                    _partialByte = (byte)(nextByte << (countOfBits - _cbitsInPartialByte));
                }

                _cbitsInPartialByte = 8 - (countOfBits - _cbitsInPartialByte);
            }
            return returnByte;
        }
    }
}
