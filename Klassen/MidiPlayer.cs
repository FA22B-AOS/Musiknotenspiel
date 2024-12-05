using NAudio.Midi;

namespace Musiknotenspiel.Klassen
{
    internal class MidiPlayer : IDisposable
    {
        private MidiOut midiOut;
        private static int channelCount = 0;

        public MidiPlayer()
        {
            midiOut = new MidiOut(0);
        }

        public async Task PlayNoteAsync(int noteNumber, int velocity = 127, int duration = 500)
        {   
            channelCount++;
            midiOut.Send(MidiMessage.StartNote(noteNumber, velocity, channelCount).RawData);

            await Task.Delay(duration);

            StopNote(noteNumber);
        }

        public void StopNote(int noteNumber)
        {
            midiOut.Send(MidiMessage.StopNote(noteNumber, 0, channelCount).RawData);
            channelCount--;
        }

        public void ChangeInstrument(int patchNumber)
        {
            if (patchNumber < 0 || patchNumber > 127)
                throw new ArgumentOutOfRangeException(nameof(patchNumber), "Patch number must be between 0 and 127.");

            midiOut.Send(MidiMessage.ChangePatch(patchNumber, 1).RawData);
        }

        public void Dispose()
        {
            midiOut?.Dispose();
        }
    }
}
