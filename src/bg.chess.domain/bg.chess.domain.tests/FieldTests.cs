using NUnit.Framework;
using System.Collections.Generic;
using System.Text;

namespace bg.chess.domain.tests
{
    public class FieldTests
    {
        [Test]
        public void GetBaseFieldTest()
        {
            var fieldBuilder = new FieldBuilder();
            var rules = new Rules();
            var field = fieldBuilder.GetField(rules);

            string fieldText = GetFieldText(field);
            var expected =
@"RNBQKBNR
PPPPPPPP
        
        
        
        
PPPPPPPP
RNBQKBNR";
            Assert.AreEqual(expected, fieldText);
        }

        [Test]
        [TestCase(3, 3, @"  K
   
K  ")]
        [TestCase(5, 3, @"    K
     
K    ")]
        [TestCase(3, 5, @"  K
   
   
   
K  ")]
        public void GetCustomFieldTest(int width, int height, string expected)
        {
            var fieldBuilder = new FieldBuilder();
            var rules = new Rules();
            rules.FieldWidth = width;
            rules.FieldHeight = height;
            rules.positions = new List<Position>
            {
                new Position(0, 0, new King(Side.White)),
                new Position(width-1, height-1, new King(Side.Black)),
            };

            var field = fieldBuilder.GetField(rules);

            string fieldText = GetFieldText(field);
            Assert.AreEqual(expected, fieldText);
        }

        private static string GetFieldText(Field field)
        {
            var sb = new StringBuilder();
            for (var h = field.FieldHeight - 1; h >= 0; h--)
            {
                for (var w = 0; w < field.FieldWidth; w++)
                {
                    sb.Append(field[w, h].Piece?.ToString() ?? " ");
                }

                if (h > 0)
                {
                    sb.AppendLine();
                }
            }

            var fieldTxt = sb.ToString();
            return fieldTxt;
        }
    }
}