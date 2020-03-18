// Solution: InformationSecurity
// Project: RC4
// RC4.cs
// Author: Nazar Lyamzin

using System.Linq;

namespace RC4
{
    public class RC4
    {
        private byte[] _s = new byte[256];
        int x = 0;
        int y = 0;

        public RC4(byte[] key)
        {
            Init(key);
        }

        // Функция меняет два элемента массива типа <T> местами
        private void Swap<T>(T[] array, int index1, int index2)
        {
            T temp = array[index1];
            array[index1] = array[index2];
            array[index2] = temp;
        }

        // Инициализацния. Алгоритм ключевого расписания
        private void Init(byte[] key)
        {
            int keyLength = key.Length;

            for (int i = 0; i < 256; i++)
            {
                _s[i] = (byte)i;
            }

            int j = 0;
            for (int i = 0; i < 256; i++)
            {
                j = (j + _s[i] + key[i % keyLength]) % 256;
                Swap(_s, i, j);
            }
        }

        private byte KeyItem()
        {
            x = (x + 1) % 256;
            y = (y + _s[x]) % 256;
            Swap(_s, x, y);
            byte K = _s[(_s[x] + _s[y]) % 256];

            return K;
        }

        // Функция шифрования/расшифрования
        public byte[] Code(byte[] text)
        {
            byte[] data = text.Take(text.Length).ToArray();
            byte[] cipher = new byte[data.Length];

            for (int i = 0; i < data.Length; i++)
            {
                cipher[i] = (byte)(data[i] ^ KeyItem());
            }
            return cipher;
        }
    }
}