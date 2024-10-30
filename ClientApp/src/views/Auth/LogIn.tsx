import React, { useContext, useState } from 'react';
import { useLocation, useNavigate } from 'react-router-dom';
import { Button, TextField, Typography, Container } from '@mui/material';
import { AuthContext } from '@/context/AuthContext';

export const LogIn = () => {
    const [username, setUsername] = useState('');

    const [password, setPassword] = useState('');
    
    const [error, setError] = useState('');
    
    const navigate = useNavigate(); // Hook to programmatically navigate

    const location = useLocation();

    const { logIn } = useContext(AuthContext);

    const queryParams = new URLSearchParams(location.search);
    const returnUrl = queryParams.get('returnUrl');
    const redirectReason = queryParams.get('redirectReason');

    const handleSubmit = async (e) => {
        e.preventDefault();
        setError(''); // Clear previous error

        try {
            // Call the login API
            const response = await logIn(
                { username, password },
                () => {
                    navigate(returnUrl || '/'); // Adjust this route as needed
                }
            );

            // Handle successful login
            if (response.status === 200) {
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
            {
                redirectReason
                    ? <Typography color='red' variant="h4" gutterBottom>{redirectReason}</Typography>
                    : null
            }
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