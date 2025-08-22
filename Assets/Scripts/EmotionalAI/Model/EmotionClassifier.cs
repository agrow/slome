namespace TL.EmotionalAI
{
    // Purpose Statement: 8-octant emotion label from the updated PAD using 0.5 thresholds.
    public static class EmotionClassifier
    {
        public static EmotionOctant From(PAD s)
        {
            int p = s.P < 0.5f ? 0 : 1, a = s.A < 0.5f ? 0 : 1, d = s.D < 0.5f ? 0 : 1;
            if (p==0 && a==0 && d==0) return EmotionOctant.Sad;
            if (p==0 && a==0 && d==1) return EmotionOctant.Resigned;
            if (p==0 && a==1 && d==0) return EmotionOctant.Anxious;
            if (p==0 && a==1 && d==1) return EmotionOctant.Angry;
            if (p==1 && a==0 && d==0) return EmotionOctant.Tender;
            if (p==1 && a==0 && d==1) return EmotionOctant.ProudSecure;
            if (p==1 && a==1 && d==0) return EmotionOctant.Awe;
            return EmotionOctant.Joy;
        }
    }
}
