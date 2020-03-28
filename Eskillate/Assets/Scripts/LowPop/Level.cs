﻿using Core;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Label = LabelHelper.Label;

namespace LowPop
{
    public class Level : ILevel
    {
        public enum Difficulty
        {
            NormalOnly,
            IntArithmetics,
            FloatArithmetics,
            ComposedExpressions
        }

        protected List<Poppable> _elements;

        public MiniGameId MiniGameId { get; set; }
        public int LevelId { get; set; }
        public Label NameLabel { get; set; }
        public Label DescriptionLabel { get; set; }
        public int HighScore { get; set; }

        private const int MAX_VALUE = 100;
        private const int MIN_VALUE = 1;
        private const int DENOMINATOR_MAX_VALUE = 10;
        protected const int SCREEN_WIDTH = 1920;
        private const int SCREEN_HEIGHT = 1080;
        private const float SPRITE_WIDTH = 153.5f;
        private const float SPRITE_HEIGHT = 153.5f;
        private const int MAX_ATTEMPTS = 100;

        private List<int> _gridOpenSlots = new List<int>();
        private int _nbSlotsOnWidth;
        private int _nbSlotsOnHeight;
        private float _slotWidth;
        private float _slotHeight;

        public Level(int nbElements, Difficulty difficulty)
        {
            _elements = new List<Poppable>();
            for (var i = 0; i < nbElements; i++)
            {
                var newElement = GenerateElement(difficulty);
                _elements.Add(newElement);
            }
        }

        private Poppable GenerateNormalElement()
        {
            var value = UnityEngine.Random.Range(MIN_VALUE, MAX_VALUE);
            return new Poppable()
            {
                Text = value.ToString(),
                Value = value
            };
        }

        private Poppable GenerateNormalFloatElement()
        {
            var value = UnityEngine.Random.Range((float)MIN_VALUE, (float)MAX_VALUE);
            return new Poppable()
            {
                Text = value.ToString(),
                Value = value
            };
        }

        private Poppable GenerateAdditionElement()
        {
            var value1 = UnityEngine.Random.Range(MIN_VALUE, MAX_VALUE/2);
            var value2 = UnityEngine.Random.Range(MIN_VALUE, MAX_VALUE/2);
            return new Poppable()
            {
                Text = value1.ToString() + " + " + value2.ToString(),
                Value = value1 + value2
            };
        }

        private Poppable GenerateAdditionFloatElement()
        {
            var value1 = UnityEngine.Random.Range((float)MIN_VALUE, (float)MAX_VALUE / 2.0f);
            var value2 = UnityEngine.Random.Range((float)MIN_VALUE, (float)MAX_VALUE / 2.0f);
            return new Poppable()
            {
                Text = value1.ToString() + " + " + value2.ToString(),
                Value = value1 + value2
            };
        }

        private Poppable GenerateSubstractionElement()
        {
            var value1 = UnityEngine.Random.Range(MIN_VALUE, MAX_VALUE);
            var value2 = UnityEngine.Random.Range(MIN_VALUE, value1);
            return new Poppable()
            {
                Text = value1.ToString() + " - " + value2.ToString(),
                Value = value1 - value2
            };
        }

        private Poppable GenerateSubstractionFloatElement()
        {
            var value1 = UnityEngine.Random.Range((float)MIN_VALUE, (float)MAX_VALUE);
            var value2 = UnityEngine.Random.Range((float)MIN_VALUE, value1);
            return new Poppable()
            {
                Text = value1.ToString() + " - " + value2.ToString(),
                Value = value1 - value2
            };
        }

        private Poppable GenerateMultiplicationElement()
        {
            var value1 = UnityEngine.Random.Range(MIN_VALUE, MAX_VALUE);
            var value2 = UnityEngine.Random.Range(MIN_VALUE, Mathf.FloorToInt(MAX_VALUE / value1));
            return new Poppable()
            {
                Text = value1.ToString() + " * " + value2.ToString(),
                Value = value1 * value2
            };
        }

        private Poppable GenerateMultiplicationFloatElement()
        {
            var value1 = UnityEngine.Random.Range((float)MIN_VALUE, (float)MAX_VALUE);
            var value2 = UnityEngine.Random.Range(MIN_VALUE, (MAX_VALUE / value1));
            return new Poppable()
            {
                Text = value1.ToString() + " * " + value2.ToString(),
                Value = value1 * value2
            };
        }

        private Poppable GenerateDivisionElement()
        {
            var denominator = UnityEngine.Random.Range(MIN_VALUE, DENOMINATOR_MAX_VALUE);
            var numerator = UnityEngine.Random.Range(MIN_VALUE, MAX_VALUE);
            var value = (float) numerator / (float) denominator;
            var roundedValue = Mathf.RoundToInt(value);
            var numeratorRounded = roundedValue * denominator;
            return new Poppable()
            {
                Text = numeratorRounded.ToString() + " / " + denominator.ToString(),
                Value = (float)numeratorRounded / (float)denominator
            };
        }

        private Poppable GenerateDivisionFloatElement()
        {
            var denominator = UnityEngine.Random.Range(MIN_VALUE, DENOMINATOR_MAX_VALUE);
            var numerator = UnityEngine.Random.Range(MIN_VALUE, MAX_VALUE);
            return new Poppable()
            {
                Text = numerator.ToString() + " / " + denominator.ToString(),
                Value = (float)numerator / (float)denominator
            };
        }

        private Poppable GenerateNegativeElement()
        {
            var value = UnityEngine.Random.Range(MIN_VALUE, MAX_VALUE);
            return new Poppable()
            {
                Text = "-" + value.ToString(),
                Value = -value
            };
        }

        private Poppable GenerateNegativeFloatElement()
        {
            var value = UnityEngine.Random.Range((float)MIN_VALUE, (float)MAX_VALUE);
            return new Poppable()
            {
                Text = "-" + value.ToString(),
                Value = -value
            };
        }

        private Poppable GenerateElement(Difficulty difficulty)
        {
            var element = new Poppable();
            
            switch(difficulty)
            {
                case Difficulty.NormalOnly:
                    element = GenerateElementNormalOnly();
                    break;
                case Difficulty.IntArithmetics:
                    element = GenerateElementIntArithmetics();
                    break;
                case Difficulty.FloatArithmetics:
                    element = GenerateElementFloatArithmetics();
                    break;
                case Difficulty.ComposedExpressions:
                    element = GenerateElementFloatArithmetics();
                    break;
                default:
                    throw new ArgumentException("This difficulty is not implemented.");
            }

            return element;
        }

        private Poppable GenerateElementNormalOnly()
        {
            bool isCorrectElement = false;
            Poppable poppable = new Poppable();
            int counter = 0;

            while (!isCorrectElement)
            {
                poppable = GenerateNormalElement();

                if (!_elements.Any(elem => elem.Value == poppable.Value))
                    isCorrectElement = true;

                if (counter >= MAX_ATTEMPTS)
                    throw new ArgumentException($"Tried {MAX_ATTEMPTS} times to generate an element, but was not sucessful. Maybe there is too many poppables or too few possible values.");

                counter++;
            }
            return poppable;
        }

        private Poppable GenerateElementIntArithmetics()
        {
            bool isCorrectElement = false;
            var operationIndex = UnityEngine.Random.Range(0, 5);
            Poppable poppable = new Poppable();
            int counter = 0;

            while (!isCorrectElement)
            {
                switch (operationIndex)
                {
                    case 0:
                        poppable = GenerateNormalElement();
                        break;
                    case 1:
                        poppable = GenerateAdditionElement();
                        break;
                    case 2:
                        poppable = GenerateSubstractionElement();
                        break;
                    case 3:
                        poppable = GenerateMultiplicationElement();
                        break;
                    case 4:
                        poppable = GenerateDivisionElement();
                        break;
                    case 5:
                        poppable = GenerateNegativeElement();
                        break;
                    default:
                        throw new ArgumentException("Tried to generate an element with an weird operationIndex");
                }

                if (!_elements.Any(elem => elem.Value == poppable.Value))
                    isCorrectElement = true;

                if (counter >= MAX_ATTEMPTS)
                    throw new ArgumentException($"Tried {MAX_ATTEMPTS} times to generate an element, but was not sucessful. Maybe there is too many poppables or too few possible values.");

                counter++;
            }
            return poppable;
        }

        private Poppable GenerateElementFloatArithmetics()
        {
            bool isCorrectElement = false;
            var operationIndex = UnityEngine.Random.Range(0, 5);
            Poppable poppable = new Poppable();
            int counter = 0;

            while (!isCorrectElement)
            {
                switch (operationIndex)
                {
                    case 0:
                        poppable = GenerateNormalFloatElement();
                        break;
                    case 1:
                        poppable = GenerateAdditionFloatElement();
                        break;
                    case 2:
                        poppable = GenerateSubstractionFloatElement();
                        break;
                    case 3:
                        poppable = GenerateMultiplicationFloatElement();
                        break;
                    case 4:
                        poppable = GenerateDivisionFloatElement();
                        break;
                    case 5:
                        poppable = GenerateNegativeFloatElement();
                        break;
                    default:
                        throw new ArgumentException("Tried to generate an element with an weird operationIndex");
                }

                if (!_elements.Any(elem => elem.Value == poppable.Value))
                    isCorrectElement = true;

                if (counter >= MAX_ATTEMPTS)
                    throw new ArgumentException($"Tried {MAX_ATTEMPTS} times to generate an element, but was not sucessful. Maybe there is too many poppables or too few possible values.");

                counter++;
            }
            return poppable;
        }

        virtual public List<Poppable> Load()
        {
            if (_elements.Count == 0)
            {
                return new List<Poppable>();
            }

            CreatePoppableGrid();

            for (int i = 0; i < _elements.Count; i++)
            {
                _elements[i].Load(i);
                var position = AssignPoppablePosition();
                _elements[i].SetPosition(position);
            }

            var poppableComparer = new PoppableComparer();
            _elements.Sort(poppableComparer);

            return new List<Poppable>(_elements);
        }

        public void Unload()
        {
            foreach(var element in _elements)
            {
                element.Unload();
            }
        }

        public List<Poppable> Reload()
        {
            return new List<Poppable>(_elements);
        }

        private void CreatePoppableGrid()
        {
            var nbElements = _elements.Count;
            // Find h/w ratio
            var ratio = (float)SCREEN_HEIGHT / SCREEN_WIDTH;
            // Find minW = sqrt(n/r)
            var minSlotsOnWidth = Mathf.Sqrt((float)nbElements / ratio);
            // Round up minW
            var minSlotsOnWidthRounded = Mathf.CeilToInt(minSlotsOnWidth);
            // Find h = r * w
            var minSlotsOnHeightRounded = Mathf.CeilToInt(ratio * minSlotsOnWidth);
            // Divide screen in w*h slots
            var nbSlots = minSlotsOnWidthRounded * minSlotsOnHeightRounded;
            // Fill openSlots with numbers from 0 to size - 1
            for(var i = 0; i < nbSlots; i++)
            {
                _gridOpenSlots.Add(i);
            }
            // Store in a private variable the number of slots in a row, the number of slots in a column, the width and the height of a slot
            _nbSlotsOnWidth = minSlotsOnWidthRounded;
            _nbSlotsOnHeight = minSlotsOnHeightRounded;

            _slotWidth = SCREEN_WIDTH / _nbSlotsOnWidth;
            _slotHeight = SCREEN_HEIGHT / _nbSlotsOnHeight;
        }

        private Vector2 AssignPoppablePosition()
        {
            // openSlotIndex = Generate a random number from 0 to size of openSlots-1
            var openSlotIndex = UnityEngine.Random.Range(0, _gridOpenSlots.Count - 1);
            // slotIndex = openSlots.At(openSlotIndex)
            var slotIndex = _gridOpenSlots[openSlotIndex];
            _gridOpenSlots.Remove(slotIndex);
            // Find the position of center of slot #slotIndex
            var slotPos = FindSlotPosition(slotIndex);
            // Find extra room in width and height in each slot (extraWidth = slot width - poppable width)
            var extraWidth = _slotWidth - SPRITE_WIDTH;
            var extraHeight = _slotHeight - SPRITE_HEIGHT;

            if (extraWidth < 0 || extraHeight < 0)
                throw new ArgumentException("Not enough space inside a slot to fit the sprite.");

            // Generate a random float between 0 and extraWidth
            var randomWidth = UnityEngine.Random.Range(0, extraWidth);
            var randomHeight = UnityEngine.Random.Range(0, extraHeight);
            // widthOffset = randomWidth - extraWidth/2
            var widthOffset = randomWidth - extraWidth / 2;
            var heightOffset = randomHeight - extraHeight / 2;
            // Return slotPosition + (widthOffset, heightOffset)
            return slotPos + new Vector2(widthOffset, heightOffset);
        }

        private Vector2 FindSlotPosition(int id)
        {
            // Find the row (id / numberSlots in a Row -> rounded down)
            var row = Mathf.FloorToInt(id / _nbSlotsOnWidth);
            var column = id - (row * _nbSlotsOnWidth);

            var x = _slotWidth / 2 + (column * _slotWidth) + -SCREEN_WIDTH / 2;
            var y = _slotHeight / 2 + (row * _slotHeight) + -SCREEN_HEIGHT / 2;

            return new Vector2(x, y);
        }
    }
}