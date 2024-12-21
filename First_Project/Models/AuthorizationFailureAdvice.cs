using Rsk.Enforcer.PEP;

[EnforcerAdvice("AuthorizationFailure")]
public class AuthorizationFailureAdvice
{
    [PolicyAttribute("Advice","AuthorizationFailureMessage")]
    public string Message { get; set; }
}