//using System;
//using System.IO;
//using System.Runtime.CompilerServices;
//using System.Security.Cryptography;

//namespace BlubLib.Security.Cryptography
//{
//    // C reference implementation https://github.com/veorq/SipHash/blob/master/siphash24.c
//    public sealed class SipHash : KeyedHashAlgorithm
//    {
//        private ulong v0;
//        private ulong v1;
//        private ulong v2;
//        private ulong v3;
//        private MemoryStream _data;
//        private BinaryReader _reader;
//        private int _processedBytes;

//        public override int HashSize => sizeof(ulong) * 8;
//        public override byte[] Key
//        {
//            get { return base.Key; }
//            set
//            {
//                if (value == null)
//                    throw new ArgumentNullException(nameof(value));

//                if (value.Length != 16)
//                    throw new ArgumentException("Key must have a length of 16 bytes");

//                base.Key = value;
//            }
//        }

//        public SipHash(byte[] key)
//        {
//            _data = new MemoryStream();
//            _reader = new BinaryReader(_data);

//            Key = key;
//            Initialize();
//        }

//        public override void Initialize()
//        {
//            var k0 = BitConverter.ToUInt64(Key, 0);
//            var k1 = BitConverter.ToUInt64(Key, 8);
//            v0 = 0x736f6d6570736575UL ^ k0;
//            v1 = 0x646f72616e646f6dUL ^ k1;
//            v2 = 0x6c7967656e657261UL ^ k0;
//            v3 = 0x7465646279746573UL ^ k1;

//            _data.Position = 0;
//            _data.SetLength(0);
//            _processedBytes = 0;
//        }

//        protected override void HashCore(byte[] array, int ibStart, int cbSize)
//        {
//            // Process 64bit blocks
//            var blocks = (_data.Length + cbSize) / 8;
//            _data.Write(array, ibStart, cbSize);
//            _data.Position = 0;

//            for (var i = 0; i < blocks; ++i)
//                Process(_reader.ReadUInt64());

//            // Remove read bytes
//            var bytesLeft = (int)_data.Length - _data.Position;
//            if (bytesLeft > 0)
//            {
//                var pos = _data.Position;
//                var buffer = _data.GetBuffer();
//                _data.Position = 0;
//                _data.Write(buffer, (int)pos, (int)bytesLeft);
//            }
//            _data.SetLength(bytesLeft);
//            _processedBytes += cbSize;
//        }

//        protected override byte[] HashFinal()
//        {
//            if (_data.Length < 8)
//                _data.SetLength(8);
//            _data.Position = 0;

//            var lastBlock = (ulong)_processedBytes << 56 | _reader.ReadUInt64();
//            Process(lastBlock);

//            v2 ^= 0xFF;

//            for (var finalizationRound = 0; finalizationRound < 4; ++finalizationRound)
//                SipRound();

//            var hash = v0 ^ v1 ^ v2 ^ v3;
//            return BitConverter.GetBytes(hash);
//        }

//        protected override void Dispose(bool disposing)
//        {
//            if (disposing)
//            {
//                if (_reader != null)
//                {
//                    _reader.Dispose();
//                    _reader = null;
//                    _data = null;
//                }
//            }
//            base.Dispose(disposing);
//        }

//        [MethodImpl(MethodImplOptions.AggressiveInlining)]
//        private void Process(ulong m)
//        {
//            v3 ^= m;
//            for (var compressionRounds = 0; compressionRounds < 2; ++compressionRounds)
//                SipRound();
//            v0 ^= m;
//        }

//        [MethodImpl(MethodImplOptions.AggressiveInlining)]
//        private void SipRound()
//        {
//            v0 += v1;
//            v1 = RotateLeft(v1, 13);
//            v1 ^= v0;
//            v0 = RotateLeft(v0, 32);
//            v2 += v3;
//            v3 = RotateLeft(v3, 16);
//            v3 ^= v2;
//            v0 += v3;
//            v3 = RotateLeft(v3, 21);
//            v3 ^= v0;
//            v2 += v1;
//            v1 = RotateLeft(v1, 17);
//            v1 ^= v2;
//            v2 = RotateLeft(v2, 32);
//        }

//        [MethodImpl(MethodImplOptions.AggressiveInlining)]
//        private static ulong RotateLeft(ulong value, int count)
//        {
//            return (value << count) | (value >> (64 - count));
//        }
//    }
//}
