using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RSA
{
    public class RSA
    {
        private UInt64 P;
        private UInt64 Q;
        private UInt64 fi;
        private UInt64 n;
        private UInt64 publicKey;
        private UInt64 privateKey;

        /// <summary>
        /// Вввод параметров P Q вручную, с проверкой на простое число
        /// </summary>
        public void GenerateKey()
        {
            do
            {
                Console.Write("Введите простое число P:\t");
                P = Convert.ToUInt64(Console.ReadLine());

                if (!IsPrime(P))
                {
                    Console.WriteLine("Ошибка. Необходимо простое число P (которое делится только на 1 или само себя)");
                }
            } while (!IsPrime(P));

            do
            {
                Console.Write("Введите простое число Q:\t");
                Q = Convert.ToUInt64(Console.ReadLine());

                if (!IsPrime(Q))
                {
                    Console.WriteLine("Ошибка. Необходимо простое число Q (которое делится только на 1 или само себя)");
                }
            } while (!IsPrime(Q));

            Console.Write("Введите экспоненту e:\t\t");
            publicKey = Convert.ToUInt64(Console.ReadLine());

            // Модуль - произведение P*Q
            n = P * Q;

            // Функция эйлера φ(n) = (p−1)⋅(q−1)
            fi = (P - 1) * (Q - 1);

            // Генерация публичной экспоненты
            GeneratePublicExp();
            // Генерация секретной экспоненты
            GenerateSecretExp();
        }

        public string ImportPublicKey()
        {
            return Convert.ToString(publicKey) + ";" + Convert.ToString(n);
        }

        public string ImportPrivateKey()
        {
            return Convert.ToString(privateKey) + ";" + Convert.ToString(n);
        }

        public void ExportPublicKey(string PublicKey)
        {
            publicKey = Convert.ToUInt64(PublicKey.Split(new char[] { ';' })[0]);
            n = Convert.ToUInt64(PublicKey.Split(new char[] { ';' })[1]);
        }

        public void ExportPrivateKey(string PrivateKey)
        {
            privateKey = Convert.ToUInt64(PrivateKey.Split(new char[] { ';' })[0]);
            n = Convert.ToUInt64(PrivateKey.Split(new char[] { ';' })[1]);
        }

        /// <summary>
        /// Шифрование текста
        /// </summary>
        /// <param name="Bytes">Массив байтов для шифрования</param>
        public byte[] Encrypt(byte []Bytes)
        {
            byte[] EncryptText = new byte[Bytes.Length];

            for (int i = 0; i < Bytes.Length; i++)
            {
                EncryptText[i] = Convert.ToByte(EncryptFunction(Bytes[i]));
            }

            return EncryptText;
        }

        /// <summary>
        /// Функция шифрования RSA
        /// </summary>
        private UInt64 EncryptFunction(byte Byte)
        {
            UInt64 input = Convert.ToUInt64(Byte);
            UInt64 result = 1;

            for (UInt64 j = 0; j < publicKey; j++)
            {
                result = result * input;
                result = result % n;
            }

            return result;
        }

        /// <summary>
        /// Рашифровка текста
        /// </summary>
        /// <param name="Bytes">Массив зашифрованных байтов</param>
        public byte[] Decrypt(byte[] Bytes)
        {
            byte[] DecryptText = new byte[Bytes.Length];

            for (int i = 0; i < Bytes.Length; i++)
            {
                DecryptText[i] = Convert.ToByte(DecryptFunction(Bytes[i]));
            }

            return DecryptText;
        }

        /// <summary>
        /// Функция расшифровки
        /// </summary>
        private UInt64 DecryptFunction(byte Byte)
        {
            UInt64 input = Convert.ToUInt64(Byte);
            UInt64 result = 1;

            for (UInt64 j = 0; j < privateKey; j++)
            {
                result = result * input;
                result = result % n;
            }

            return result;
        }

        /// <summary>
        /// Проверка простого числа
        /// </summary>
        private bool IsPrime(UInt64 Number)
        {
            UInt64 N = Convert.ToUInt64(Math.Sqrt(Convert.ToDouble(Number)));
            for (UInt64 i = 2; i < N; i++)
            {
                if (Number % i == 0)
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Проверка двух чисел на взаимопростоту
        /// </summary>
        private bool IsPrime(UInt64 Number1, UInt64 Number2)
        {
            if ((Number2 % Number1) == 0)
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// Генерация публичной экспоненты (e)
        /// </summary>
        /// <returns></returns>
        private void GeneratePublicExp()
        {
            // Проверка экспоненты (e) со значением функции Эйлера (fi) на взаимнопростое
            if (IsPrime(publicKey, fi))
            {
                return;
            }
            
            Console.WriteLine("Экспонента и значение функции Эйлера не являетюся взаимопростыми числами.\n" +
                                  "Сгенерирована новая экспонента");

            // Генерируем новую экспоненту
            while (!IsPrime(publicKey) || !IsPrime(publicKey, fi))
            {
                publicKey = Convert.ToUInt64(new Random().Next(2, Convert.ToInt32(fi) - 1));
            }
        }

        /// <summary>
        /// Генерация секретной экспоненты (d)
        /// </summary>
        private void GenerateSecretExp()
        {
            UInt64 k = 1;

            // Вычисляется число d, удовлетворяющее сравнению: d * e ≡ 1 (mod φ(n))
            while (true)
            {
                k = k + fi;

                if (k % publicKey == 0)
                {
                    privateKey = (k / publicKey);
                    return;
                }
            }
        }
    }
}
