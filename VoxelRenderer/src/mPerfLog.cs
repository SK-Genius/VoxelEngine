using System.Runtime.InteropServices;
using System.Text;

public static class
mPerfLog {
	
	private static readonly System.IntPtr PseudoHandle = -2;
	
	[DllImport("kernel32.dll")]
	private static extern bool
	QueryThreadCycleTime(
		System.IntPtr aThreadHandle,
		out tNat64 aCycles
	);
	
	private static tInt32 NextIndex;
	
	public struct
	tPerfLog : System.IDisposable {
		public tText Region;
		
		public void
		Dispose(
		) {
			QueryThreadCycleTime(PseudoHandle, out var Tick);
			Logs[NextIndex] = new tLogLine{
				Region = this.Region,
				Type = tLogType.End,
				Ticks = Tick,
			};
			NextIndex += 1;
			NextIndex &= cBufferMask;
		}
	}
	
	private enum
	tLogType {
		Begin,
		End,
	}
	
	private struct
	tLogLine {
		public tLogType Type;
		public tText Region;
		public tNat64 Ticks;
	}
	
	private const tInt32 cBufferAdressBits = 16;
	private const tInt32 cBufferSize = 1 << cBufferAdressBits;
	private const tInt32 cBufferMask = cBufferSize - 1;
	private static tLogLine[] Logs = new tLogLine[cBufferSize];
	
	public static tPerfLog
	LogPerf(
		[CallerMemberName]tText aRegion = ""
	) {
		QueryThreadCycleTime(PseudoHandle, out var Tick);
		Logs[NextIndex] = new tLogLine{
			Type = tLogType.Begin,
			Region = aRegion,
			Ticks = Tick
		};
		NextIndex += 1;
		NextIndex &= cBufferMask;
		return new tPerfLog {
			Region = aRegion,
		};
	}
	
	public static tText
	Print(
	) {
		var Index = 0;
		var Result = new StringBuilder();
		
		while (Index < NextIndex) {
			Result.ReadRegion(ref Index, 0);
		}
		
		return Result.ToString();
	}
	
	private static StringBuilder ReadRegion(
		this StringBuilder aResult,
		ref tInt32 aIndex,
		tInt32 aIntend
	) {
		var Begin = Logs[aIndex];
		aIndex += 1;
		aResult.Append('\t', aIntend).Append(Begin.Region).Append(" {").AppendLine();
		while (Logs[aIndex].Type == tLogType.Begin) {
			aResult.ReadRegion(ref aIndex, aIntend + 1);
		}
		var End = Logs[aIndex];
		aIndex += 1;
		aResult.Append('\t', aIntend).Append("} ").AppendFormat((End.Ticks - Begin.Ticks).ToString("#,##0")).AppendLine();
		
		return aResult;
	}
} 
