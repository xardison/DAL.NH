namespace DAL.NH.Filter
{
    public interface IPager
    {
        int Totalltems { get; }
        int PageCurrent { get; }
        int PageSize { get; }
        int PageTotal { get; }
        int PageStart { get; }
        int PageEnd { get; }

        void RecalcPages(int totalltems = -1);
    }
}