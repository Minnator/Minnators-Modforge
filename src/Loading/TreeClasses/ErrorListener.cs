//using Antlr4.Runtime;
//using Editor.ErrorHandling;
//using Editor.Saving;

//namespace Editor.Loading.TreeClasses
//{
//   public class MyErrorListener(ref PathObj path) : BaseErrorListener
//   {
//      private PathObj Path = path;

//      public override void SyntaxError(
//         TextWriter output,
//         IRecognizer recognizer,
//         IToken offendingSymbol,
//         int line,
//         int charPositionInLine,
//         string msg,
//         RecognitionException e)
//      {
//         lock (this)
//         {
//            _ = new LoadingError(Path, msg, line, charPositionInLine);
//         }
//      }

//   }

//}