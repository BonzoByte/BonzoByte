// server.js
import cors from 'cors';
import 'dotenv/config';
import express, { json, urlencoded } from 'express';
import helmet from 'helmet';
import compression from 'compression';
import morgan from 'morgan';
import rateLimit from 'express-rate-limit';
import path from 'path';
import passport from 'passport';
import { fileURLToPath } from 'url';
import { MulterError } from 'multer';

import { env, corsAllowlist } from './config/env.js';
import connectDB from './config/db.js';
import './config/passport.js';

import countryRoutes from './routes/country.routes.js';
import playsRoutes from './routes/plays.routes.js';
import surfaceRoutes from './routes/surface.routes.js';
import tournamentTypeRoutes from './routes/tournamentType.routes.js';
import tournamentLevelRoutes from './routes/tournamentLevel.routes.js';
import tournamentEventRoutes from './routes/tournamentEvent.routes.js';
import playerRoutes from './routes/player.routes.js';
import matchRoutes from './routes/match.routes.js';
import authRoutes from './routes/auth.routes.js';
import userRoutes from './routes/user.routes.js';
import archivesRoutes from './routes/archives.routes.js';

const app = express();
app.set('trust proxy', 1);

const __filename = fileURLToPath(import.meta.url);
const __dirname = path.dirname(__filename);

/* ----------------------------- C O R S  ----------------------------- */
// ⚠️ CORS MORA BITI ispred SVIH ruta i većine middlewara
const corsOptions = {
    origin(origin, cb) {
        const ok = !origin || corsAllowlist.includes(origin);
        cb(ok ? null : new Error('Not allowed by CORS'), ok);
    },
    methods: ['GET', 'POST', 'PUT', 'PATCH', 'DELETE', 'OPTIONS', 'HEAD'],   // 👈 PATCH dodan
    allowedHeaders: ['Origin', 'X-Requested-With', 'Content-Type', 'Accept', 'Authorization'], // 👈 Authorization
    credentials: true,
    optionsSuccessStatus: 204,
};
app.use(cors(corsOptions));
// Preflight za sve rute
app.options(/.*/, cors(corsOptions));

/* --------------------------- Core middleware ------------------------ */
app.use(helmet({
    crossOriginResourcePolicy: { policy: 'cross-origin' }, // 👈 dopusti embed s 4200
    crossOriginEmbedderPolicy: false,                      // 👈 ne traži COEP u devu
}));
app.use(compression());
app.use(morgan('dev'));
app.use(json({ limit: '1mb' }));
app.use(urlencoded({ extended: true }));

/* --------------------------- Rate limiting -------------------------- */
// Ne brojaj preflight; u devu želimo mir
const skipPreflight = (req) => req.method === 'OPTIONS';

const globalLimiter = rateLimit({
    windowMs: 15 * 60 * 1000,
    max: 1000,
    standardHeaders: true,
    legacyHeaders: false,
    skip: skipPreflight,          // 👈
});
app.use(globalLimiter);

const authLimiter = rateLimit({
    windowMs: 10 * 60 * 1000,
    max: 50,
    message: { message: 'Too many auth requests, try again later.' },
    standardHeaders: true,
    legacyHeaders: false,
    skip: skipPreflight,          // 👈
});

/* ----------------------------- Passport ----------------------------- */
app.use(passport.initialize());

/* ------------------------------ Statics ----------------------------- */
app.use('/uploads', express.static(path.join(__dirname, 'uploads'), {
    setHeaders: (res) => {
        res.setHeader('Cross-Origin-Resource-Policy', 'cross-origin');
    }
}));

/* -------------------------------- Routes ---------------------------- */
app.use('/api/countries', countryRoutes);
app.use('/api/plays', playsRoutes);
app.use('/api/surfaces', surfaceRoutes);
app.use('/api/tournamentTypes', tournamentTypeRoutes);
app.use('/api/tournamentLevels', tournamentLevelRoutes);
app.use('/api/tournamentEvents', tournamentEventRoutes);
app.use('/api/players', playerRoutes);
app.use('/api/matches', matchRoutes);
app.use('/api/auth', authLimiter, authRoutes);

// 👇 FRONTEND gađa /api/users/updateUserProfile → montiramo PLURAL /users
app.use('/api/users', userRoutes);
app.use('/api/archives', archivesRoutes);

/* --------------------------- Error handlers ------------------------- */
app.use((err, req, res, next) => {
    // Multer upload greške
    if (err instanceof MulterError) {
        if (err.code === 'LIMIT_FILE_SIZE') {
            return res.status(400).json({ message: 'File too large (max 2MB).' });
        }
        return res.status(400).json({ message: `Upload error: ${err.code}` });
    }
    // CORS greška iz origin callbacka
    if (err && err.message === 'Not allowed by CORS') {
        return res.status(403).json({ message: err.message });
    }
    // Invalid file type iz fileFiltera
    if (err && err.message === 'INVALID_FILE_TYPE') {
        return res.status(400).json({ message: 'Invalid file type. Allowed: JPG, PNG, WebP.' });
    }
    next(err);
});

// health & 404
app.get('/api/health', (_, res) => res.json({ ok: true }));
app.use((req, res) => res.status(404).json({ message: 'Ruta nije pronađena' }));

/* --------------------------- Start server --------------------------- */
await connectDB();
app.listen(env.PORT, () => console.log(`Server running on :${env.PORT} 🚀`));

process.on('unhandledRejection', e => { console.error(e); process.exit(1); });
process.on('uncaughtException', e => { console.error(e); process.exit(1); });