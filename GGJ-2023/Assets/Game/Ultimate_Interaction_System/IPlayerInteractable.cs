public interface IPlayerInteractable
{
	void Hover();
	void UnHover();
	void Select();
	void UnSelect();
	bool IsSelectable { get; }
}
