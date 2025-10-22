namespace ConsoleApp1;

public class Lane
{
    public int left { get; set; }
    public int right { get; set; }

    public Lane(int left, int right)
    {
        this.left = left;
        this.right = right;
    }
    
    public override bool Equals(object obj)
    {
        if (obj is Lane other)
        {
            return this.left == other.left && this.right == other.right;
        }
        return false;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(left, right);
    }

        


}
