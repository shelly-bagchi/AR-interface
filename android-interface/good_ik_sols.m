function angles = good_ik_sols(theta)
angles = [];
for i = 1:size(theta, 2)
    if theta(2,i) < 0 && theta(2,i) >= -pi/2 && theta(3,i) > 0
        angles =[angles, theta(:,i)];
    elseif theta(2,i) < -pi/2 && theta(2,i) >= -pi  && theta(3,i) < 0
        angles = [angles, theta(:,i)];
    else
        angles = [angles, []];

    end
    
end
    






end