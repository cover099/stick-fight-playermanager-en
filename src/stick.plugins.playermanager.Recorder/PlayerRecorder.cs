namespace stick.plugins.playermanager.Recorder;

public class PlayerRecorder : RecorderBase
{
	protected override string FileName => "CustomPlayerRecord.txt";

	protected override PlayerRecordStatus RecordStatus => PlayerRecordStatus.Recorded;
}
