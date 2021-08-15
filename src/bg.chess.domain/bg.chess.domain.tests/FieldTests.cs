using NUnit.Framework;
using System.Collections.Generic;
using System.Text;

namespace Bg.Chess.Domain.Tests
{
    public class FieldTests
    {
        [Test]
        public void GetBaseFieldTest()
        {
            var field = new Field().WithDefaultRules();
            string fieldText = GetFieldText(field);
            var expected =
@"rnbqkbnr
pppppppp
        
        
        
        
PPPPPPPP
RNBQKBNR";
            Assert.AreEqual(expected, fieldText);
        }

        [Test]
        [TestCase(3, 3, @"  k
   
K  ")]
        [TestCase(5, 3, @"    k
     
K    ")]
        [TestCase(3, 5, @"  k
   
   
   
K  ")]
        public void GetCustomFieldTest(int width, int height, string expected)
        {
            var rules = new ClassicRules();
            rules.FieldWidth = width;
            rules.FieldHeight = height;
            rules.Positions = new List<Position>
            {
                new Position(0, 0, PieceBuilder.King(Side.White)),
                new Position(width-1, height-1, PieceBuilder.King(Side.Black)),
            };

            var field = new Field(rules);

            string fieldText = GetFieldText(field);
            Assert.AreEqual(expected, fieldText);
        }

        private string GetFieldText(Field field)
        {
            var sb = new StringBuilder();
            for (var h = field.FieldHeight - 1; h >= 0; h--)
            {
                for (var w = 0; w < field.FieldWidth; w++)
                {
                    var piece = field[w, h].Piece;
                    if (piece == null)
                    {
                        sb.Append(" ");
                    }
                    else
                    {
                        var name = piece.Type.ShortName;
                        if (piece.Side == Side.White)
                        {
                            sb.Append(name.ToString().ToUpper());
                        }
                        else
                        {
                            sb.Append(name);
                        }
                    }
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