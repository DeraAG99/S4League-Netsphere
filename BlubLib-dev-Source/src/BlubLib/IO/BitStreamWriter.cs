using System;
using System.Collections.Generic;

namespace BlubLib.IO
{
    // From http://referencesource.microsoft.com/#PresentationCore/Shared/MS/Internal/Ink/BitStream.cs,a9f766f17e02d469
    public class BitStreamWriter
    {
        private readonly IList<byte> _targetBuffer;
        private int _remaining;

        public BitStreamWriter(IList<byte> bufferToWriteTo)
        {
            if (bufferToWriteTo == null)
                throw new ArgumentNullException(nameof(bufferToWriteTo));

            _targetBuffer = bufferToWriteTo;
        }

        public void Write(uint bits, int countOfBits)
        {
            // validate that a subset of the bits in a single byte are being written
            if (countOfBits <= 0 || countOfBits > 32)
                throw new ArgumentOutOfRangeException(nameof(countOfBits));


            // calculate the number of full bytes 
            //   Example: 10 bits would require 1 full byte 
            var fullBytes = countOfBits / 8;

            // calculate the number of bits that spill beyond the full byte boundary
            //   Example: 10 buttons would require 2 extra bits (8 fit in a full byte)
            var bitsToWrite = countOfBits % 8;

            for (; fullBytes >= 0; fullBytes--)
            {
                var byteOfData = (byte)(bits >> (fullBytes * 8));
                //
                // write 8 or less bytes to the bitwriter 
                // checking for 0 handles the case where we're writing 8, 16 or 24 bytes
                // and bitsToWrite is initialize to zero
                //
                if (bitsToWrite > 0)
                    Write(byteOfData, bitsToWrite);
                if (fullBytes > 0)
                    bitsToWrite = 8;
            }
        }

        public void WriteReverse(uint bits, int countOfBits)
        {
            // validate that a subset of the bits in a single byte are being written 
            if (countOfBits <= 0 || countOfBits > 32)
                throw new ArgumentOutOfRangeException(nameof(countOfBits));

            // calculate the number of full bytes
            //   Example: 10 bits would require 1 full byte 
            var fullBytes = countOfBits / 8;

            // calculate the number of bits that spill beyond the full byte boundary
            //   Example: 10 buttons would require 2 extra bits (8 fit in a full byte) 
            var bitsToWrite = countOfBits % 8;
            if (bitsToWrite > 0)
            {
                fullBytes++;
            }
            for (var x = 0; x < fullBytes; x++)
            {
                var byteOfData = (byte)(bits >> (x * 8));
                Write(byteOfData, 8);
            }
        }

        public void Write(byte bits, int countOfBits)
        {
            // validate that a subset of the bits in a single byte are being written 
            if (countOfBits <= 0 || countOfBits > 8)
                throw new ArgumentOutOfRangeException(nameof(countOfBits));

            byte buffer;
            // if there is remaining bits in the last byte in the stream
            //      then use those first
            if (_remaining > 0)
            {
                // retrieve the last byte from the stream, update it, and then replace it 
                buffer = _targetBuffer[_targetBuffer.Count - 1];
                // if the remaining bits aren't enough then just copy the significant bits
                //      of the input into the remainder 
                if (countOfBits > _remaining)
                {
                    buffer |= (byte)((bits & (0xFF >> (8 - countOfBits))) >> (countOfBits - _remaining));
                }
                // otherwise, copy the entire set of input bits into the remainder
                else
                {
                    buffer |= (byte)((bits & (0xFF >> (8 - countOfBits))) << (_remaining - countOfBits));
                }
                _targetBuffer[_targetBuffer.Count - 1] = buffer;
            }

            // if the remainder wasn't large enough to hold the entire input set 
            if (countOfBits > _remaining)
            {
                // then copy the uncontained portion of the input set into a temporary byte 
                _remaining = 8 - (countOfBits - _remaining);
                unchecked // disable overflow checking since we are intentionally throwing away 
                          //  the significant bits
                {
                    buffer = (byte)(bits << _remaining);
                }
                // and add it to the target buffer
                _targetBuffer.Add(buffer);
            }
            else
            {
                // otherwise, simply update the amount of remaining bits we have to spare
                _remaining -= countOfBits;
            }
        }
    }
}
