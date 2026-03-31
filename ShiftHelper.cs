using BarcodeBartenderApp;

public static class ShiftHelper
{
    public static string GetCurrentShift()
    {
        var shifts = DatabaseHelper.GetShifts();
        TimeSpan now = DateTime.Now.TimeOfDay;

        foreach (var s in shifts)
        {
            if (s.start < s.end)
            {
                // Normal shift
                if (now >= s.start && now < s.end)
                    return s.shift;
            }
            else
            {
                // Night shift (cross midnight)
                if (now >= s.start || now < s.end)
                    return s.shift;
            }
        }

        return "Unknown";
    }
}