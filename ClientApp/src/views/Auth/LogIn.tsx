import React, { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { Button, TextField, Typography, Container } from '@mui/material';
import { CentaureaApi } from '@/api/CentaureaApi'; // Adjust the import path as needed

export const LogIn = () => {
    const [username, setUsername] = useState('');

    const [password, setPassword] = useState('');
    
    const [error, setError] = useState('');
    
    const navigate = useNavigate(); // Hook to programmatically navigate

    const handleSubmit = async (e) => {
        e.preventDefault();
        setError(''); // Clear previous error

        try {
            // Call the login API
            const response = await CentaureaApi.logIn({ username, password });

            // Handle successful login
            if (response.status === 200) {
                // Redirect to the desired page, e.g., dashboard
                // navigate('/'); // Adjust this route as needed
            } else {
                setError('Invalid username or password');
            }
        } catch (err) {
            setError('Login failed. Please try again.');
            console.error('[Error]', err);
        }
    };

    return (
        <Container maxWidth="xs">
            <Typography variant="h4" gutterBottom>Log In</Typography>
            <form onSubmit={handleSubmit}>
                <TextField
                    label="Username"
                    variant="outlined"
                    fullWidth
                    margin="normal"
                    value={username}
                    onChange={(e) => setUsername(e.target.value)}
                    required
                />
                <TextField
                    label="Password"
                    type="password"
                    variant="outlined"
                    fullWidth
                    margin="normal"
                    value={password}
                    onChange={(e) => setPassword(e.target.value)}
                    required
                />
                {error && <Typography color="error">{error}</Typography>}
                <Button type="submit" variant="contained" color="primary" fullWidth>
                    Log In
                </Button>
            </form>
        </Container>
    );
};