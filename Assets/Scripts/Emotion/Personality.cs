public class PersonalityProfile
{
    //okay to use full names for enums here
    public enum Energy { Introverted, Extraverted }


    public enum Mind { Observant, Intuitive }
    public enum Nature { Thinking, Feeling }
    public enum Tactics { Judging, Prospecting }
    public enum Identity { Assertive, Turbulent }

    private Energy energy;
    private Mind mind;
    private Nature nature;
    private Tactics tactics;
    private Identity identity;


// for accessing later in case we need (ro)
    public Energy EnergyTrait => energy;
    public Mind MindTrait => mind;
    public Nature NatureTrait => nature;
    public Tactics TacticsTrait => tactics;
    public Identity IdentityTrait => identity;

    public string MBTIType => GenerateMBTIType();


//basic constructor for now 
    public PersonalityProfile(Energy energy, Mind mind, Nature nature, Tactics tactics, Identity identity)
    {
        this.energy = energy;
        this.mind = mind;
        this.nature = nature;
        this.tactics = tactics;
        this.identity = identity;
    }

    // private method to generate the MBTI type string// for logging for now?
    private string GenerateMBTIType()
    {
        string mbti = "";
        mbti += (energy == Energy.Extraverted) ? "E" : "I";
        mbti += (mind == Mind.Intuitive) ? "N" : "S";
        mbti += (nature == Nature.Thinking) ? "T" : "F";
        mbti += (tactics == Tactics.Judging) ? "J" : "P";
        mbti += "-" + ((identity == Identity.Assertive) ? "A" : "T");
        return mbti;
    }
}
