using System;
using NUnit.Framework;

namespace MiniAudio.Interop.Tests {

    public class MiniAudioHandlerTests {

        [SetUp]
        public void SetUp() {
            ConstantImports.Initialize();
        }

        [Test]
        public void LoadSoundBinding() {
            var actual = LibraryHandler.GetDelegate<MiniAudioLoadHandler>(ConstantImports.MiniAudioHandle, "LoadSound");
            Assert.AreNotEqual(IntPtr.Zero, actual);
        }

        [TearDown]
        public void TearDown() {
            ConstantImports.Release();
        }
    }
}
