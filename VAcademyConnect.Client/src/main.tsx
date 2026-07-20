import { StrictMode } from 'react'
import { createRoot } from 'react-dom/client'
import { ConfigProvider } from 'antd'
import '@ant-design/v5-patch-for-react-19'
import './index.css'
import App from './App.tsx'

const theme = {
  token: {
    colorPrimary: '#4f46e5',
    colorSuccess: '#16a34a',
    colorWarning: '#d97706',
    colorError: '#dc2626',
    colorInfo: '#4f46e5',
    colorTextBase: '#0f172a',
    colorBorder: '#e2e8f0',
    colorBgLayout: '#f8fafc',
    borderRadius: 8,
    fontFamily: "'Inter', sans-serif",
  },
  components: {
    Table: {
      headerBg: '#f8fafc',
      headerColor: '#334155',
      borderColor: '#e2e8f0',
      rowHoverBg: '#eef2ff',
    },
    Menu: {
      darkItemBg: 'transparent',
      darkItemSelectedBg: '#4f46e5',
      darkItemHoverBg: 'rgba(255, 255, 255, 0.06)',
    },
    Card: {
      borderRadiusLG: 12,
    },
    Button: {
      controlHeight: 38,
    },
  },
}

createRoot(document.getElementById('root')!).render(
  <StrictMode>
    <ConfigProvider theme={theme}>
      <App />
    </ConfigProvider>
  </StrictMode>,
)
