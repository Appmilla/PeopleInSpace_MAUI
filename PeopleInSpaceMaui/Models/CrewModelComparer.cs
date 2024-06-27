namespace PeopleInSpaceMaui.Models;

public class CrewModelComparer : IEqualityComparer<CrewModel>
{
    public bool Equals(CrewModel? x, CrewModel? y)
    {
        if (x == null && y == null) return true;
        if (x == null || y == null) return false;

        return x.Id == y.Id && 
               x.Name == y.Name && 
               x.Agency == y.Agency && 
               x.Image == y.Image && 
               x.Wikipedia == y.Wikipedia &&
               x.Launches.SequenceEqual(y.Launches) &&
               x.Status == y.Status;
    }

    public int GetHashCode(CrewModel? obj)
    {
        if (obj == null) return 0;

        var hashCode = obj.Id.GetHashCode();
        hashCode = (hashCode * 397) ^ obj.Name.GetHashCode();
        hashCode = (hashCode * 397) ^ obj.Agency.GetHashCode();
        hashCode = (hashCode * 397) ^ obj.Image.GetHashCode();
        hashCode = (hashCode * 397) ^ obj.Wikipedia.GetHashCode();
        hashCode = (hashCode * 397) ^ obj.Launches.GetHashCode();
        hashCode = (hashCode * 397) ^ obj.Status.GetHashCode();

        return hashCode;
    }
}
