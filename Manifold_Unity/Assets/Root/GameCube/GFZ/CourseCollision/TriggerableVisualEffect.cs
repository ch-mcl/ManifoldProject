namespace GameCube.GFZ.CourseCollision
{
    [System.Flags]
    public enum TriggerableVisualEffect : ushort
    {
        Lightning_NoRain = 1 << 0,
        BigBlue_WaterSplash = 1 << 0,
        OuterSpace_Unknown = 1 << 0,

        SandOcean_SandstormVfx = 1 << 1,

        GreenPlant_LeavesVfx1 = 1 << 4,
        GreenPlant_LeavesVfx2 = 1 << 5,
        GreenPlant_LeavesVfx3 = 1 << 7,

        OuterSpace_NoAshes = 1 << 12,

        DimLighting = 1 << 13,
        BigBlueStory_DimLighting = 1 << 13,
        SandOcean_DimLighting = 1 << 13,

        AlternateFog = 1 << 14,
        BigBlue_AlternateFog = 1 << 14,
        SandOcean_AlternateFog = 1 << 14,
        FireFieldStory_AlternateFog = 1 << 14,
    }
}
