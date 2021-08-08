using NUnit.Framework;
using System.Text;

namespace bg.chess.domain.tests
{
    public class FieldTests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void GetBaseFieldsTest()
        {
            var fieldBuilder = new FieldBuilder();
            var rules = new Rules();
            var field = fieldBuilder.GetField(rules);

            var sb = new StringBuilder();
            for (var h = 0; h < rules.FieldHeight; h++)
            {
                for (var w = 0; w < rules.FieldWidth; w++)
                {
                    sb.Append(field[w, h].Piece?.ToString() ?? " ");
                }

                if (h < rules.FieldHeight - 1)
                {
                    sb.AppendLine();
                }
            }

            var fieldTxt = sb.ToString();
            var expected = 
@"RNBQKBNR
PPPPPPPP
        
        
        
        
PPPPPPPP
RNBQKBNR";
            Assert.AreEqual(expected, fieldTxt);
        }
    }
}