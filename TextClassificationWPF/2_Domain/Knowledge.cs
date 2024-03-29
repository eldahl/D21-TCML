﻿using System.Collections.Generic;
using System.Data;
using System.Windows.Documents;

namespace TextClassificationWPF.Domain
{
    // composite object - the complete "brain" of the app
    public class Knowledge
    {
        private Dictionary<string, List<string>>? _fileLists;
        private BagOfWords? _bagOfWords;
        private Vectors? _vectors;

        public Knowledge() {

        }

        public BagOfWords GetBagOfWords() {
            // Warnings...
            if (_bagOfWords is null)
                throw new DataException();
            
            return _bagOfWords;
        }
        public Dictionary<string, List<string>> GetFileLists() {
            // Warnings...
            if (_fileLists is null)
                throw new DataException();

            return _fileLists;
        }

        public Vectors GetVectors() {
            // Warnings...
            if (_vectors is null)
                throw new DataException();

            return _vectors;
        }
        public void SetFileLists(Dictionary<string, List<string>> fileLists) {
            _fileLists = fileLists;
        }

        public void SetBagOfWords(BagOfWords bagOfWords) {
            _bagOfWords = bagOfWords;
        }

        public void SetVectors(Vectors vectors) {
            _vectors = vectors;
        }

    }
}
