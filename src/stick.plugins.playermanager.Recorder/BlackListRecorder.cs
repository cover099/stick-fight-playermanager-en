namespace stick.plugins.playermanager.Recorder;

public class BlackListRecorder : RecorderBase
{
	protected override string FileName => "BlackList.txt";

	protected override PlayerRecordStatus RecordStatus => PlayerRecordStatus.Blacklisted;
}
