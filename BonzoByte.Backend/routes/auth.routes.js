import express from 'express';
import passport from 'passport';
import path from 'path';
import fs from 'fs';
import multer from 'multer';
import { v4 as uuidv4 } from 'uuid';

import User from '../models/user.model.js';
import protect from '../middleware/auth.middleware.js';
import generateToken from '../utils/generateToken.js';
import {
    registerUser, loginUser, logoutUser, verifyEmail, forgotPassword, resetPassword,
    resendVerificationEmail, contactUs,
    requestResetPassword, updateUserProfile
} from '../controllers/auth.controller.js';
import rateLimit from 'express-rate-limit';

const router = express.Router();

const FRONTEND = process.env.FRONTEND_URL || 'http://localhost:4200';
const UPLOADS_DIR = path.join(process.cwd(), 'uploads');
if (!fs.existsSync(UPLOADS_DIR)) fs.mkdirSync(UPLOADS_DIR, { recursive: true });

// --- multer setup ---
const storage = multer.diskStorage({
    destination: (_, __, cb) => cb(null, UPLOADS_DIR),
    filename: (_, file, cb) => cb(null, uuidv4() + path.extname(file.originalname))
});
const upload = multer({
    storage,
    fileFilter: (_, file, cb) => {
        const ok = ['image/jpeg', 'image/png'].includes(file.mimetype);
        cb(ok ? null : new Error('Only .jpg and .png formats are allowed.'), ok);
    },
    limits: { fileSize: 2 * 1024 * 1024 } // 2MB
});
const withUpload = (req, res, next) =>
    upload.single('avatar')(req, res, err => err ? res.status(400).json({ message: err.message }) : next());

// --- helper: sanitize user payload for client ---
const sanitizeUser = (u) => u && ({
    _id: u._id,
    name: u.name,
    email: u.email,
    nickname: u.nickname,
    avatarUrl: u.avatarUrl,
    isAdmin: u.isAdmin,
    isUser: u.isUser,
    isVerified: u.isVerified,
    provider: u.provider,
});

const contactLimiter = rateLimit({ windowMs: 15 * 60 * 1000, max: 10 });

// --- core auth ---
router.post('/register', registerUser);
router.post('/login', loginUser);
router.post('/logout', protect, logoutUser);

router.post('/verify-email', verifyEmail);
router.post('/forgotPassword', forgotPassword);
router.post('/reset-password', resetPassword);
router.post('/request-reset-password', requestResetPassword);
router.post('/resend-verification', resendVerificationEmail);
router.post('/contact', contactLimiter, contactUs)

// CHANGED: /me – vrati trenutno prijavljenog korisnika (JWT u Authorization)
router.get('/me', protect, (req, res) => {
    return res.json(sanitizeUser(req.user));
});

router.post('/update', protect, withUpload, updateUserProfile);

// --- OAuth: Google ---
router.get('/google', (req, res, next) =>
    passport.authenticate('google', {
        scope: ['profile', 'email'],
        session: false,
        state: req.query.state
    })(req, res, next)
);
router.get(
    '/google/callback',
    passport.authenticate('google', {
        session: false,
        failureRedirect: `${FRONTEND}/auth/callback?error=oauth_failed`
    }),
    issueJwtHandler
);

// --- OAuth: Facebook ---
router.get('/facebook', (req, res, next) =>
    passport.authenticate('facebook', {
        scope: ['email', 'public_profile'],
        session: false,
        state: req.query.state
    })(req, res, next)
);
router.get(
    '/facebook/callback',
    passport.authenticate('facebook', {
        session: false,
        failureRedirect: `${FRONTEND}/auth/callback?error=oauth_failed`
    }),
    issueJwtHandler
);

// --- nickname check ---
router.get('/nickname-exists/:nickname', async (req, res) => {
    try {
        const existing = await User.findOne({ nickname: req.params.nickname });
        res.json(Boolean(existing));
    } catch {
        res.json(false);
    }
});

// --- izdavanje JWT-a nakon uspješne OAuth autentikacije ---
function issueJwtHandler(req, res) {
    if (!req.user) return res.status(401).json({ message: 'Unauthorized' });
    const token = generateToken(req.user._id);

    // CHANGED: redirect na Angular rutu koju imaš u app.routes.ts
    return res.redirect(`${FRONTEND}/oauth-success?token=${encodeURIComponent(token)}`);
}

export default router;