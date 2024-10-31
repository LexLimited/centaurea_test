import React from 'react';
import {
    Container,
    Typography,
    Button,
    Grid,
    Paper,
    Box,
    Grid2,
} from '@mui/material';
import { AuthContext } from '@/context/AuthContext';
import { Link } from 'react-router-dom';

export const Index = () => {
    const { authStatus } = React.useContext(AuthContext);
    const isAuthenticated = Boolean(authStatus.username); // Check if the user is authenticated

    return (
        <Container maxWidth="md" sx={{ width: '100%', marginTop: 4 }}>
            {isAuthenticated ? (
                <>
                    <Typography variant="h4" component="h1" gutterBottom>
                        You: {authStatus.username}
                    </Typography>
                    <Typography variant="body1" gutterBottom>
                        Welcome to the grid editing app.
                    </Typography>
                </>
            ) : (
                <>
                    <Typography color='red' variant="h5" component="h2" gutterBottom>
                        Authenticate to access this application.
                    </Typography>
                    <Link to="/auth">
                        <Button
                            variant="contained"
                            color="primary"
                            size="large"
                            sx={{
                                borderRadius: '20px',
                                padding: '16px',
                                marginTop: 2,
                                width: 250
                            }}
                        >
                            Authenticate
                        </Button>
                    </Link>
                </>
            )}
        </Container>
    );
};
