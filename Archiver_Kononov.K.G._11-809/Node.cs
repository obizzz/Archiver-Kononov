namespace Archiver_Kononov.K.G._11_809
{
    public class Node
    {
        public char? Symbol;
        public int Frequency;
        
        public Node LeftChild;
        public Node RightChild;

        public Node(char? symbol = null, int frequency = 0)
        {
            Symbol = symbol;
            Frequency = frequency;
        }
    }
}