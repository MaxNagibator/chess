using NUnit.Framework;
using System.Collections.Generic;
using System.Text;

namespace Bg.Chess.Domain.Tests
{
    public class GameTests
    {
        /// <summary>
        /// ������� ����, ��� �� ����� ������
        /// </summary>
        /// <remarks>
        /// Slav Defense
        /// ���������� ������ ��������� � �������� ������� 
        /// � ��������� �� ����� ����� ����� 
        /// 1.d2->d4 d7->d5 
        /// 2.c2->c4 c7->c6.
        /// ������� ���������� �� ������ � ����� �� �������� ������, 
        /// ����� ���� ���������� �������� �������� ����� ����� �������������. 
        /// ����� �� ������� �����������, �� �� �������� ������������ ����������, 
        /// ���������� � ������ � �������� ��������� �������������� 
        /// ����������� ����������� ����������� ������� ������.</remarks>
        [Test]
        public void GameWithDefaultRulesTest()
        {
            var game = new Game();
            game.Init();
            game.Move(Side.White, "d2", "d4");
            game.Move(Side.Black, "d7", "d5");
            game.Move(Side.White, "c2", "c4");
            game.Move(Side.Black, "c7", "c6");

            // �� ���������� ����� �����, ����� � �������, ���� �������
            //�������� ��� ������ �����

        }
    }
}