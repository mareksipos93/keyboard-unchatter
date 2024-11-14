using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace KeyboardUnchatter
{
    public class DataGridController
    {
        public class KeyData
        {
            private Keys _key;
            private int _keyPressedCount = 0;
            private int _keyBlockedCount = 0;

            public Keys Key
            {
                get => _key;
            }

            public int KeyBlockedCount
            {
                get => _keyBlockedCount;
                set => _keyBlockedCount = value;
            }

            public int KeyPressedCount
            {
                get => _keyPressedCount;
                set => _keyPressedCount = value;
            }

            public KeyData(Keys key)
            {
                _key = key;
            }
        }

        private DataGridView _dataGrid;

        Dictionary<int, KeyData> _keyDataStatus = new Dictionary<int, KeyData>();

        public DataGridController(DataGridView dataGrid)
        {
            _dataGrid = dataGrid;
            _dataGrid.DoubleBuffered(true);
        }

        public void AddKeyPress(Keys key)
        {
            KeyData keyData;

            if(!_keyDataStatus.TryGetValue((int)key,out keyData))
            {
                keyData = new KeyData(key);

                _keyDataStatus[(int)key] = keyData;
            }

            _keyDataStatus[(int)key].KeyPressedCount++;

            UpdateGridCell(keyData);
        }

        public void AddKeyBlock(Keys key)
        {
            KeyData keyData;

            if (!_keyDataStatus.TryGetValue((int)key, out keyData))
            {
                keyData = new KeyData(key);

                _keyDataStatus[(int)key] = keyData;
            }

            _keyDataStatus[(int)key].KeyBlockedCount++;

            UpdateGridCell(keyData);
        }

        private void UpdateGridCell(KeyData keyData)
        {
            bool cellUpdated = false;
            var keyPressed = keyData.Key.ToString();
            var keyDisabled = Properties.Settings.Default.disabledKeys.Contains(keyPressed);
            int press = keyData.KeyPressedCount;
            int block = keyData.KeyBlockedCount;
            var percentage = Math.Floor((float)block / (float)press * 100).ToString() + "%";

            for (int a = 0; a < _dataGrid.Rows.Count; ++a)
            {
                var row = _dataGrid.Rows[a];
                var keyColumnCell = row.Cells[1];

                if (keyColumnCell.Value != null && keyColumnCell.Value.ToString() == keyPressed)
                {
                    _dataGrid.Rows[a].SetValues(new object[] { keyDisabled, keyPressed, press.ToString(), block.ToString(), percentage});
                    cellUpdated = true;
                    break;
                }
            }

            if (!cellUpdated)
            {
                _dataGrid.Rows.Add(new object[] { keyDisabled, keyPressed, press.ToString(), block.ToString(), percentage });
            }
        }
    }
}
