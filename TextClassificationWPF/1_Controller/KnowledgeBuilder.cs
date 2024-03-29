﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Windows.Markup;
using TextClassificationWPF.Business;
using TextClassificationWPF.Domain;
using TextClassificationWPF.FileIO;
using static System.Net.Mime.MediaTypeNames;

namespace TextClassificationWPF.Controller
{
    public class KnowledgeBuilder : AbstractKnowledgeBuilder
    {
        private Knowledge _knowledge; // the composite object

        private Dictionary<string, List<string>>? _fileLists;
        private BagOfWords? _bagOfWords;
        private Vectors? _vectors;

        private List<string>? categoryNames;

        public KnowledgeBuilder(List<string> folderNames) {
            _knowledge = new Knowledge();

            categoryNames = folderNames;

            if (categoryNames is null)
                throw new ArgumentNullException();
        }

        public List<string> GetCategoryNames() {
            if (categoryNames is null)
                throw new ArgumentException("Category names are null.");

            return categoryNames;
        }

        public override void Train() {
            // (1) 
            BuildFileLists();
            // (2)
            BuildBagOfWords();
            // (3)
            BuildVectors();
        }
        
        public override void BuildFileLists() {

            FileListBuilder flb = new FileListBuilder();

            // If categoryNames was not set we cannot continue.
            if (categoryNames is null)
                throw new ArgumentException();

            // Add all filenames for each category
            foreach (string s in categoryNames)
                flb.GenerateFileNames(s);

            _fileLists = flb.GetFileLists();
            _knowledge.SetFileLists(_fileLists);
        }
        
        public override void BuildBagOfWords() {
            if (_fileLists == null) {
                BuildFileLists();
            }
            _bagOfWords = new BagOfWords();

            // If categoryNames was not set we cannot continue.
            if (categoryNames is null)
                throw new ArgumentException();

            // Add all categoryNames to BagOfWords
            foreach (string s in categoryNames)
                AddToBagOfWords(s);

            _knowledge.SetBagOfWords(_bagOfWords);
        }

        public override void BuildVectors() {
            if (_fileLists == null) {
                BuildFileLists();
            }
            if (_bagOfWords == null) {
                BuildBagOfWords();
            }

            // If categoryNames was not set we cannot continue.
            if (categoryNames is null)
                throw new ArgumentException();

            _vectors = new Vectors();
            
            // Add vectors from categoryNames
            foreach(string s in categoryNames)
                AddToVectors(s);

            _knowledge.SetVectors(_vectors);
        }

        private void AddToBagOfWords(string folderName) {
            List<string>? list;

            if (_fileLists is null)
                throw new ArgumentNullException();
            if (_bagOfWords is null)
                throw new ArgumentNullException();

            _fileLists.TryGetValue(folderName, out list);

            if (list is null)
                throw new ArgumentNullException();

            int tokenCount = 0;
            for (int i = 0; i < list.Count; i++) {
                string text = File.ReadAllText(list[i]);

                List<string> wordsInFile = Tokenization.Tokenize(text);
                tokenCount += wordsInFile.Count;

                foreach (string word in wordsInFile)
                    _bagOfWords.InsertEntry(word);
            }
        }

        private void AddToVectors(string folderName) {
            List<string>? fileList;

            if (_fileLists is null)
                throw new ArgumentNullException();
            if (_bagOfWords is null)
                throw new ArgumentNullException();
            if (_vectors is null)
                throw new ArgumentNullException();

            _fileLists.TryGetValue(folderName, out fileList);

            if (fileList is null)
                throw new ArgumentNullException();

            for (int i = 0; i < fileList.Count; i++)
                _vectors.AddVector(folderName, CreateVector(File.ReadAllText(fileList[i])));
        }

        public List<bool> CreateVector(string text) {
            if (_bagOfWords is null)
                throw new ArgumentNullException();

            List<bool> vector = new List<bool>();
            List<string> wordsInFile = Tokenization.Tokenize(text);
            foreach (string key in _bagOfWords.GetAllWordsInDictionary()) {
                if (wordsInFile.Contains(key))
                    vector.Add(true);
                else
                    vector.Add(false);
            }
            return vector;
        }

        public override Knowledge GetKnowledge() {
            return _knowledge;
        }
    }
}
