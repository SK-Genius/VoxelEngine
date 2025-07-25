using System;

using static mV2;

public static class
mEvents {
	public interface iEvent { }
	
	public record
	tMouseMove(
		tV2 NewPos,
		tV2 OldPos
	) : iEvent {
	}
	
	[Flags]
	public enum
	tKeys {
		None = 0,
		
		MouseLeft = 1 << 0,
		MouseMiddle = 1 << 1,
		MouseRight = 1 << 2,
		
		Shift = 1 << 3,
		Control = 1 << 4,
		Alt = 1 << 5,
		
		Up = 1 << 6,
		Down = 1 << 7,
		Left = 1 << 8,
		Right = 1 << 9,
	}
	
	public record
	tKeyDown(
		tKeys Key
	) : iEvent {
	}
	
	public record
	tKeyUp(
		tKeys Key
	) : iEvent {
	}
	
	public record
	tMouseScroll(
		tInt32 Key
	) : iEvent {
	}
}
