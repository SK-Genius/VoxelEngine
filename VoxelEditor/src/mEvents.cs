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
	tMouseKeys {
		Left = 1 << 0,
		Middle = 1 << 1,
		Right = 1 << 2,
	}
	
	public record
	tMouseKeyDown(
		tMouseKeys Key
	) : iEvent {
	}
	
	public record
	tMouseKeyUp(
		tMouseKeys Key
	) : iEvent {
	}
	
	public record
	tMouseScroll(
		tInt32 Key
	) : iEvent {
	}
}
