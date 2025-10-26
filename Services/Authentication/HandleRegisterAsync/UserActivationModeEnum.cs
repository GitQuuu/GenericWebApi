namespace Services.Authentication.HandleRegisterAsync;

/// <summary>
/// Defines the modes in which a user account can be activated.
/// </summary>
public enum UserActivationModeEnum
{
	/// <summary>
	/// Represents an activation mode where the user is responsible for activating
	/// their own account, typically through a confirmation process such as
	/// clicking a link sent via email.
	/// </summary>
	SelfActivation,

	/// <summary>
	/// Represents an activation mode where a user's account requires explicit
	/// approval by an administrator before it becomes active.
	/// </summary>
	AdminApproval,

	/// <summary>
	/// Represents an activation mode where user accounts are activated automatically
	/// without requiring any manual action from the user or an administrator.
	/// </summary>
	AutoActivation,
}