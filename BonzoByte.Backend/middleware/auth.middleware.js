import jwt from 'jsonwebtoken';
import User from '../models/user.model.js';

const protect = async (req, res, next) => {
    try {
        const auth = req.headers.authorization || '';
        if (!auth.startsWith('Bearer ')) {
            return res.status(401).json({ message: 'Nema tokena. Prijava potrebna.' });
        }
        const token = auth.split(' ')[1];
        const decoded = jwt.verify(token, process.env.JWT_SECRET);
        const user = await User.findById(decoded.id).select('-password');
        if (!user) return res.status(401).json({ message: 'Korisnik ne postoji.' });
        req.user = user;
        next();
    } catch {
        return res.status(401).json({ message: 'Neautoriziran pristup.' });
    }
};

export const isUser = (req, res, next) => req.user?.isUser ? next() : res.status(403).json({ message: 'User role required' });
export const isAdmin = (req, res, next) => req.user?.isAdmin ? next() : res.status(403).json({ message: 'Admin access only' });

export default protect;