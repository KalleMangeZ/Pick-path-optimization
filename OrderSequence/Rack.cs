namespace ConsoleApp1;
 
public class Rack{
    public int laneNbr { get; set; }
    public int rackNbr { get; set; }
 
    public Rack(int rackNbr, int laneNbr) {
        this.rackNbr = rackNbr;
        this.laneNbr = laneNbr;
    }
}