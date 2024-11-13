namespace Editor.Helper
{
   public static class MathHelper
   {
      //TODO MELCO: Optimize this method :)
      /// <summary>
      /// This method splits a value into n random pieces with the sum of the pieces being equal to the value and the average of the pieces being equal to the value/n
      /// </summary>
      /// <param name="numberOfSplits"></param>
      /// <param name="valueToSplit"></param>
      /// <returns></returns>
      public static List<int> SplitIntoNRandomPieces(int numberOfSplits, int valueToSplit, int minValue, int maxValue)
      {
         if (numberOfSplits <= 0 || valueToSplit <= 0 || minValue <= 0 || maxValue <= 0 || minValue >= maxValue)
         {
            MessageBox.Show("numberOfSplits, valueToSplit, minValue, and maxValue must be positive, and minValue must be less than maxValue", "Invalid input values.", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return [];
         }

         if (valueToSplit < numberOfSplits * minValue || valueToSplit > numberOfSplits * maxValue)
         {
            MessageBox.Show("The value to split must be between numberOfSplits * minValue and numberOfSplits * maxValue", "Invalid input values.", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return [];
         }

         List<int> pieces = [];
         var remainingValue = valueToSplit;

         // Calculate the ideal size for each piece
         var idealPieceSize = valueToSplit / numberOfSplits;

         // First, distribute the ideal size to all pieces, while respecting the min/max bounds
         for (var i = 0; i < numberOfSplits; i++)
         {
            // Start with the ideal piece size
            var pieceSize = idealPieceSize;

            // Adjust within the allowed range to avoid exceeding min/max
            var maxAdjustment = Math.Min(pieceSize - minValue, maxValue - pieceSize);
            if (maxAdjustment > 0) 
               pieceSize += Globals.Random.Next(-maxAdjustment, maxAdjustment + 1);

            // Ensure the piece stays within the min and max bounds
            pieceSize = Math.Max(pieceSize, minValue);
            pieceSize = Math.Min(pieceSize, maxValue);

            // Add the piece size and update the remaining value
            pieces.Add(pieceSize);
            remainingValue -= pieceSize;
         }

         // If there's any remaining value, adjust the pieces to make the total sum equal to valueToSplit
         var difference = remainingValue;
         for (var i = 0; difference != 0; i = (i + 1) % numberOfSplits)
         {
            var adjustment = Math.Sign(difference);
            var newPieceSize = pieces[i] + adjustment;

            // Ensure the new size stays within the min/max bounds
            if (newPieceSize >= minValue && newPieceSize <= maxValue)
            {
               pieces[i] = newPieceSize;
               difference -= adjustment;
            }
         }

         return pieces;
      }




   }
}