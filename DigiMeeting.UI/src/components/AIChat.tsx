import React, { useState } from 'react';
import axios from 'axios';

const AIChat: React.FC = () => {
  const [userInput, setUserInput] = useState('');
  const [messages, setMessages] = useState<{ text: string; isUser: boolean }[]>([]);

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    
    // Replace with your actual backend endpoint URL
    const response = await axios.post('https://your-backend-api.com/endpoint', {
      message: userInput
    });

    // Handle the response from the backend (e.g., update state with AI response)
    setMessages(prev => [
      ...prev,
      { text: userInput, isUser: true },
      { text: 'AI: ' + response.data.message, isUser: false }
    ]);

    setUserInput('');
  };

  return (
    <div style={{ maxWidth: '600px', margin: 'auto', padding: '20px' }}>
      <h2>AI Chat</h2>
      <div style={{ border: '1px solid #ccc', padding: '10px', maxHeight: '400px', overflowY: 'auto' }}>
        {messages.map((msg, index) => (
          <div key={index} style={{ marginBottom: '10px' }}>
            <strong>{msg.isUser ? 'You' : 'AI'}:</strong> {msg.text}
          </div>
        ))}
      </div>
      <form onSubmit={handleSubmit} style={{ display: 'flex', marginTop: '10px' }}>
        <input
          type="text"
          value={userInput}
          onChange={(e) => setUserInput(e.target.value)}
          placeholder="Type your message..."
          style={{ flex: 1, padding: '8px' }}
        />
        <button type="submit" style={{ marginLeft: '5px', padding: '8px 15px' }}>Send</button>
      </form>
    </div>
  );
};

export default AIChat;