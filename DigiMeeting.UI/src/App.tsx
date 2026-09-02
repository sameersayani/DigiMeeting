import { BrowserRouter as Router, Routes, Route, Navigate } from 'react-router-dom';
import { RegisterPage } from './components/RegisterPage';
import { LoginPage } from './components/LoginPage';
import { BookingCalendar } from './components/BookingCalendar';
import AIChat from './components/AIChat';

function App() {
  return (
    <div>
      <Router>
        <Routes>
          <Route path="/register" element={<RegisterPage />} />
          <Route path="/login" element={<LoginPage />} />
          <Route path="/dashboard" element={<BookingCalendar />} />
          <Route path="/" element={<Navigate to="/login" />} />
        </Routes>
      </Router>
      <AIChat />
    </div>
  );
}

export default App;
